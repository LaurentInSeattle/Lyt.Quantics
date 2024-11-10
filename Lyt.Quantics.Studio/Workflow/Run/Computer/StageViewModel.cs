using Lyt.Quantics.Studio.Workflow.Run.Gates;

namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public sealed class StageViewModel : Bindable<StageView>
{
    public readonly int stageIndex;
    public readonly QsModel quanticsStudioModel;
    public readonly IToaster toaster;

    private int activeGates;

    public StageViewModel(int stageIndex, QsModel quanticsStudioModel)
    {
        this.stageIndex = stageIndex;
        this.quanticsStudioModel = quanticsStudioModel;
        this.Name = (1 + stageIndex).ToString();
        this.IsSelected = true;
        this.toaster = App.GetRequiredService<IToaster>();
    }

    public bool IsEmpty => this.activeGates == 0;

    public bool IsSelected { get; private set; }

    public void UpdateGatesAndMinibars()
    {
        if (this.Control is null)
        {
            // Too early: The View might is still null 
            return;
        }

        this.UpdateUiGates();
        this.UpdateUiMinibars();
    }

    public void Select(bool select)
    {
        this.IsSelected = select;
        this.IsMarkerVisible = select;
    }

    public bool CanDrop(Point point, IGateInfoProvider gateInfoProvider)
    {
        // TODO: This prevents moving a gate in the circuit view !
        if (!gateInfoProvider.IsToolbox)
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
        if (gateInfoProvider.Gate.MatrixDimension > 2)
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

    public void OnDrop(Point point, IGateInfoProvider gateInfoProvider)
    {
        if (!this.CanDrop(point, gateInfoProvider))
        {
            return;
        }

        // 600 pixels for 10 qubits ~ Magic numbers !
        int qubitIndex = (int)Math.Floor(point.Y / 60.0);
        this.AddGateAt(new QubitsIndices(qubitIndex), gateInfoProvider.Gate);
    }

    public void AddGateAt(QubitsIndices qubitsIndices, Gate gate)
    {
        var computer = this.quanticsStudioModel.QuComputer;
        int gateQubits = gate.QuBitsTransformed;
        foreach (int qubitIndex in qubitsIndices.AllQubitIndicesSorted())
        {
            if (gateQubits + qubitIndex > computer.QuBitsCount)
            {
                this.toaster.Show(
                    "Can't Drop Here!",
                    "Not enough qubits to drop the gate here.",
                    4_000, InformationLevel.Warning);
                return;
            }
        }

        if (!this.quanticsStudioModel.AddGate(this.stageIndex, qubitsIndices, gate, out string message))
        {
            this.toaster.Show("Failed to Add Gate!", message, 4_000, InformationLevel.Error);
            return;
        }
    }

    private void UpdateUiGates()
    {
        try
        {
            var computer = this.quanticsStudioModel.QuComputer;
            if (this.stageIndex >= computer.Stages.Count)
            {
                // this is the last empty UI stage used to drop new gates 
                // Nothing to update in this one, just bail out
                return;
            }

            this.activeGates = 0;
            this.View.GatesGrid.Children.Clear();
            var stage = computer.Stages[this.stageIndex];
            if (stage.Operators.Count == 0)
            {
                // Or nothing to do actually 
                return;
            }

            foreach (var stageOperator in stage.Operators)
            {
                string gateKey = stageOperator.GateKey;
                if (stageOperator.GateKey == IdentityGate.Key)
                {
                    // No need to show the identity operator
                    continue;
                }

                ++this.activeGates;

                var gate = GateFactory.Produce(gateKey, stageOperator.GateParameters);
                int gateRows = gate.QuBitsTransformed;
                int firstIndex = stageOperator.SmallestQubitIndex;
                var qubitsIndices = new QubitsIndices(stageOperator);

                if (ConstructedGateViewModel.IsGateSupported(gateKey))
                {
                    var gateVm = 
                        new ConstructedGateViewModel(gateKey, this.stageIndex, qubitsIndices);
                    var gateView = gateVm.CreateViewAndBind();
                    gateView.SetValue(Grid.RowProperty, firstIndex);
                    gateView.SetValue(Grid.RowSpanProperty, gateRows);
                    this.View.GatesGrid.Children.Add(gateView);
                }
                else
                {
                    // TODO ~ TODO : Take care of Control and Targets 

                    var gateViewModel =
                        new GateViewModel(
                            gate, isToolbox: false, stageIndex: this.stageIndex, qubitIndex: firstIndex);
                    var view = gateViewModel.CreateViewAndBind();
                    view.SetValue(Grid.RowProperty, firstIndex);
                    view.SetValue(Grid.RowSpanProperty, gateRows);
                    this.View.GatesGrid.Children.Add(view);

                    // TODO ~ TODO : Take care of Control and Targets 
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "UI Gates Update: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    public void UpdateUiMinibars()
    {
        try
        {
            this.View.MinibarsGrid.Children.Clear();
            if (this.quanticsStudioModel.HideMinibarsUserOption ||
                this.quanticsStudioModel.HideMinibarsComputerState)
            {
                return;
            }

            var computer = this.quanticsStudioModel.QuComputer;
            if (!computer.IsComplete)
            {
                Debug.WriteLine("Cant update minibars: computer not complete");
                return;
            }

            if (this.stageIndex >= computer.Stages.Count)
            {
                // This should never happen 
                if (this.stageIndex > computer.Stages.Count)
                {
                    if (Debugger.IsAttached) { Debugger.Break(); }
                }

                // This is the last empty stage used to drop gates: nothing to do 
                return;
            }

            var stage = computer.Stages[this.stageIndex];
            var probabilities = stage.QuBitProbabilities;
            if (probabilities.Count != computer.QuBitsCount)
            {
                string uiMessage = "Mismatch between qubit probabilities count and Qubit count.";
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
