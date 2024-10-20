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

    public bool IsEmpty()
    {
        int activeGates = 0;
        foreach (var gate in this.Gates)
        {
            if (gate is not null)
            {
                ++activeGates;
            }
        }

        return activeGates == 0;
    }

    public bool CanDrop(Point point, GateViewModel gateViewModel)
    {
        // TODO: This prevents moving a gate !
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

        // Can't drop a binary or ternary gate on last qubit 
        if (gateViewModel.Gate.Dimension > 2)
        {
            var computer = this.quanticsStudioModel.QuComputer;
            int qubitIndex = (int)Math.Floor(offset);
            if (qubitIndex >= computer.QuBitsCount - 1)
            {
                return false;
            }
        }

        return true;
    }

    public void OnDrop(Point point, GateViewModel gateViewModel)
    {
        if (!this.CanDrop(point, gateViewModel))
        {
            return;
        }

        // 600 pixels for 10 qubits ~ Magic numbers !
        int qubitIndex = (int)Math.Floor(point.Y / 60.0);
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
            if (this.stageIndex >= computer.Stages.Count)
            {
                // this is the last empty UI stage used to drop new gates 
                // Nothing to update in this one, just bail out
                return;
            }

            var stage = computer.Stages[this.stageIndex];
            foreach (var stageOperator in stage.Operators)
            {
                if (stageOperator.GateKey == IdentityGate.Key)
                {
                    // No need to show the identity operator
                    continue;
                }

                int firstIndex = stageOperator.QuBitIndices.Min();
                var gate = GateFactory.Produce(stageOperator.GateKey);
                int gateRows = gate.Dimension / 2;
                var gateViewModel =
                    new GateViewModel(
                        gate, isToolbox: false, stageIndex: this.stageIndex, qubitIndex: firstIndex);
                this.Gates[firstIndex] = gateViewModel;
                var view = gateViewModel.CreateViewAndBind();
                view.SetValue(Grid.RowProperty, firstIndex);
                view.SetValue(Grid.RowSpanProperty, gateRows);
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
            if (probabilities.Count != computer.QuBitsCount)
            {
                string uiMessage = "Mismatch between probabilities count and Qubit count.";
                this.toaster.Show("Error", uiMessage, 4_000, InformationLevel.Error);
                return;
            }

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
