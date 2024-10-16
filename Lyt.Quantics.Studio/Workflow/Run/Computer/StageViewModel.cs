namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public sealed class StageViewModel : Bindable<StageView>
{
    public readonly int stageIndex;
    public readonly QuanticsStudioModel quanticsStudioModel;
    public readonly IToaster toaster;

    public StageViewModel(int stageIndex, QuanticsStudioModel quanticsStudioModel)
    {
        this.stageIndex = stageIndex;
        this.quanticsStudioModel = quanticsStudioModel;
        this.Gates = new GateViewModel?[ComputerViewModel.MaxQubits];
        this.Name = (1 + stageIndex).ToString();
        this.IsSelected = true;
        this.toaster = App.GetRequiredService<IToaster>();
    }

    public bool IsSelected { get; private set; }

    public GateViewModel?[] Gates { get; private set; }

    public void UpdateOnQubitRemoved(int removedQubitIndex)
    {
        this.UpdateUiGates();
        this.UpdateUiMinibars();
    }

    public void Update()
    {
        this.UpdateUiGates();
        this.UpdateUiMinibars();
    }

    public void Select(bool select)
    {
        this.IsSelected = select;
        this.IsMarkerVisible = select;
    }

    public bool CanDrop(Point point, GateViewModel gateViewModel)
    {
        if (!gateViewModel.IsToolbox)
        {
            return false;
        }

        // 600 pixels for 10 qubits ~ Magic numbers !
        double offset = point.Y / 60.0;
        if ((offset < 0.0) || (offset >= 10.0))
        {
            // outside qubit area: reject
            return false;
        }

        return true;
    }

    public void OnDrop(Point point, GateViewModel gateViewModel)
    {
        if (!gateViewModel.IsToolbox)
        {
            return;
        }

        // 600 pixels for 10 qubits ~ Magic numbers !
        double offset = point.Y / 60.0;
        if ((offset < 0.0) || (offset >= 10.0))
        {
            // outside qubit area: reject
            return;
        }

        int qubitIndex = (int)Math.Floor(offset);
        this.AddGateAt(qubitIndex, gateViewModel.Gate);
    }

    private void AddGateAt(int qubitIndex, Gate gate)
    {
        if (!this.quanticsStudioModel.AddGate(this.stageIndex, qubitIndex, gate, out string message))
        {
            this.toaster.Show("Failed to Add Gate!", message, 4_000, InformationLevel.Error);
            return;
        }
    }

    private void UpdateUiGates()
    {
        try
        {
            this.View.GatesGrid.Children.Clear();
            var computer = this.quanticsStudioModel.QuComputer;
            var stage = computer.Stages[this.stageIndex];
            foreach (var stageOperator in stage.Operators)
            {
                int firstIndex = stageOperator.QuBitIndices.Min();
                var gate = GateFactory.Produce(stageOperator.GateKey);
                var gateViewModel =
                    new GateViewModel(
                        gate, isToolbox: false, stageIndex: this.stageIndex, qubitIndex: firstIndex);
                var view = gateViewModel.CreateViewAndBind();
                view.SetValue(Grid.RowProperty, firstIndex);
                this.View.GatesGrid.Children.Add(view);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "UI Gates Update: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void UpdateUiMinibars()
    {
        try
        {
            this.View.MinibarsGrid.Children.Clear();
            var computer = this.quanticsStudioModel.QuComputer;
            if (!computer.IsComplete)
            {
                return;
            }

            var stage = computer.Stages[this.stageIndex];
            var probabilities = stage.Probabilities;
            for (int qubitIndex = 0; qubitIndex < computer.QuBitsCount; qubitIndex++)
            {
                double value = probabilities.At(qubitIndex);
                var vm = new AmplitudeMinibarViewModel(value);
                var view = vm.CreateViewAndBind();
                view.SetValue(Grid.RowProperty, qubitIndex);
                this.View.MinibarsGrid.Children.Add(view);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "UI Minibars Update: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public bool IsMarkerVisible { get => this.Get<bool>(); set => this.Set(value); }
}
