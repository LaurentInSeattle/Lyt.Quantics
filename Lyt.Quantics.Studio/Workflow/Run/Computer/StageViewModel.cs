﻿namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public sealed partial class StageViewModel : ViewModel<StageView>, IDropTarget
{
    private const double MarginHeight = 30.0;
    private const double QuBitHeight = 60.0;
    private const double OtherHeights = 70.0;

    public readonly int stageIndex;
    public readonly QsModel quanticsStudioModel;
    public readonly IToaster toaster;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private bool isMarkerVisible;

    [ObservableProperty]
    private double gridHeight;

    private int activeGates;

    public StageViewModel(int stageIndex, QsModel quanticsStudioModel)
    {
        this.stageIndex = stageIndex;
        this.quanticsStudioModel = quanticsStudioModel;
        this.Name = (1 + stageIndex).ToString();
        this.IsSelected = true;
        this.toaster = App.GetRequiredService<IToaster>();
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.UpdateQubitCount();
    }

    private static double PixelsFromQuBitsCount(int quBitsCount)
        => OtherHeights + QuBitHeight * quBitsCount;

    public bool IsEmpty => this.activeGates == 0;

    public bool IsSelected { get; private set; }

    public void UpdateQubitCount()
    {
        if (!this.IsBound)
        {
            return;
        }

        var view = this.View;
        int qubitCount = this.quanticsStudioModel.QuComputer.QuBitsCount;
        this.GridHeight = PixelsFromQuBitsCount(qubitCount);
        for (int rowIndex = 0; rowIndex < QuRegister.MaxQubits; ++rowIndex)
        {
            var gateRow = view.GatesGrid.RowDefinitions[rowIndex];
            var barRow = view.MinibarsGrid.RowDefinitions[rowIndex];
            GridLength rowHeight = new(rowIndex < qubitCount ? QuBitHeight : 0.0, GridUnitType.Pixel);
            gateRow.Height = rowHeight;
            barRow.Height = rowHeight;
        }
    }

    public void UpdateGatesAndMinibars()
    {
        if (!this.IsBound)
        {
            // Too early: The View might still be null 
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

    public bool CanDrop(Point point, object droppedObject)
    {
        if (droppedObject is not IGateInfoProvider gateInfoProvider)
        {
            Debug.WriteLine("Invalid drop object");
            return false;
        }

        int quBitIndex = this.ToQuBitIndex(point);
        if (quBitIndex == -1)
        {
            // outside qubit area: reject
            return false;
        }

        // Can't drop a binary or ternary gate on last qubit 
        if (!gateInfoProvider.Gate.IsUnary)
        {
            var computer = this.quanticsStudioModel.QuComputer;
            if (quBitIndex >= computer.QuBitsCount - 1)
            {
                return false;
            }
        }

        return true;
    }

    public void OnDrop(Point point, object droppedObject)
    {
        if (droppedObject is not IGateInfoProvider gateInfoProvider)
        {
            return;
        }

        if (!this.CanDrop(point, gateInfoProvider))
        {
            return;
        }

        int quBitIndex = this.ToQuBitIndex(point);
        if (quBitIndex == -1)
        {
            // outside qubit area: reject
            return;
        }

        if (gateInfoProvider.IsToolbox)
        {
            this.AddGateAt(new QubitsIndices(quBitIndex), gateInfoProvider.Gate, isDrop: true);
        }
        else
        {
            // We are moving a gate in the circuit view !
            // Debug.WriteLine("Removing gate: " + gateInfoProvider.Gate.CaptionKey);
            if (!this.quanticsStudioModel.RemoveGate(
                gateInfoProvider.StageIndex, gateInfoProvider.QubitsIndices, out string message))
            {
                this.toaster.Show(
                    "Failed to Remove gate: " + gateInfoProvider.Gate.CaptionKey, message,
                    4_000, InformationLevel.Error);
            }
            else
            {
                this.AddGateAt(new QubitsIndices(quBitIndex), gateInfoProvider.Gate, isDrop: true);
            }
        }
    }

    public void HideDropTarget(DropTargetControl dropTargetView)
    {
        if (!this.IsBound)
        {
            return;
        }

        var children = this.View.GatesGrid.Children;
        if (children.Contains(dropTargetView))
        {
            children.Remove(dropTargetView);
        }
    }

    public void ShowDropTarget(DropTargetControl dropTargetView, Point point)
    {
        // Debug.WriteLine("ShowDropTarget");
        if (!this.IsBound)
        {
            // Debug.WriteLine("ShowDropTarget: Not Bound");
            return;
        }

        var parent = dropTargetView.Parent;
        if ((parent is not null) && (parent != this.View.GatesGrid))
        {
            // Debug.WriteLine("Abort: not our target view");
            return;
        }

        // 60 pixels for each qubits ~ Magic number!
        var children = this.View.GatesGrid.Children;
        int quBitIndex = this.ToQuBitIndex(point);
        if (quBitIndex == -1)
        {
            // outside qubit area: reject
            // Debug.WriteLine("ShowDropTarget: Reject");
            return;
        }

        // Debug.WriteLine("Drop View at index: " + qubitIndex.ToString());
        dropTargetView.SetValue(Grid.RowProperty, quBitIndex);

        if (!children.Contains(dropTargetView))
        {
            // Debug.WriteLine("Drop View Added");
            children.Add(dropTargetView);
        }
    }

    public void AddGateAt(QubitsIndices qubitsIndices, Gate gate, bool isDrop)
    {
        var computer = this.quanticsStudioModel.QuComputer;
        foreach (int qubitIndex in qubitsIndices.AllQubitIndicesSorted())
        {
            if (qubitIndex >= computer.QuBitsCount)
            {
                this.toaster.Show(
                    "Can't Drop Here!",
                    "Not enough qubits to drop the gate here.",
                    4_000, InformationLevel.Warning);
                return;
            }
        }

        if ( isDrop)
        {
            this.HideDropTarget(StageView.DropTargetView);
        }

        if (!this.quanticsStudioModel.AddGate(
                this.stageIndex, qubitsIndices, gate, isDrop, out string message))
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
            var grid = this.View.GatesGrid; 
            grid.Children.Clear();

            if (this.stageIndex >= computer.Stages.Count)
            {
                // this is the last empty UI stage used to drop new gates 
                // Nothing to update in this one, just bail out
                return;
            }

            this.activeGates = 0;
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

                var gateParameters = stageOperator.GateParameters; 
                var gate = GateFactory.Produce(gateKey, gateParameters);
                int firstIndex = stageOperator.SmallestQubitIndex;
                int lastIndex = stageOperator.LargestQubitIndex;
                int gateRows = 1 + lastIndex - firstIndex;
                var qubitsIndices = new QubitsIndices(stageOperator);

                UserControl gateView; 

                // Must check this one first as many constructed supported gates as also 
                // of type ControlledGate 
                if (ConstructedGateViewModel.IsGateSupported(gateKey))
                {
                    var gateVm =
                        new ConstructedGateViewModel(gateKey, this.stageIndex, qubitsIndices);
                    gateView = gateVm.CreateViewAndBind();
                }
                else if (gate is ControlledGate controlledGate)
                {
                    var gateVm =
                        new ControlledGateViewModel(gateParameters, this.stageIndex, qubitsIndices);
                    gateView = gateVm.CreateViewAndBind();
                }
                else
                {
                    var gateViewModel =
                        new GateViewModel(
                            gate, isToolbox: false, stageIndex: this.stageIndex, qubitIndex: firstIndex);
                    gateView = gateViewModel.CreateViewAndBind();
                }

                gateView.SetValue(Grid.RowProperty, firstIndex);
                gateView.SetValue(Grid.RowSpanProperty, gateRows);
                grid.Children.Add(gateView);
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
            if (!this.IsBound)
            {
                // Too early: The View might still be null 
                return;
            }

            this.View.MinibarsGrid.Children.Clear();
            if (this.quanticsStudioModel.HideMinibarsUserOption ||
                this.quanticsStudioModel.HideMinibarsComputerState)
            {
                return;
            }

            var computer = this.quanticsStudioModel.QuComputer;
            if (!computer.IsComplete)
            {
                // Debug.WriteLine("Cant update minibars: computer not complete");
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
            double[] probabilities = stage.QuBitProbabilities;
            if (probabilities.Length != computer.QuBitsCount)
            {
                string uiMessage = "Mismatch between qubit probabilities count and Qubit count.";
                this.toaster.Show("Error", uiMessage, 4_000, InformationLevel.Error);
                return;
            }

            for (int qubitIndex = 0; qubitIndex < computer.QuBitsCount; qubitIndex++)
            {
                var vm = new AmplitudeMinibarViewModel(probabilities[qubitIndex]);
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

    private int ToQuBitIndex(Point point)
    {
        double y = point.Y - MarginHeight;
        int qubitIndex = (int)Math.Floor(y / QuBitHeight);
        var computer = this.quanticsStudioModel.QuComputer;
        if ((qubitIndex < 0) || (qubitIndex >= computer.QuBitsCount))
        {
            // outside qubit area: reject
            return -1;
        }
        else
        {
            return qubitIndex;
        }
    }
}
