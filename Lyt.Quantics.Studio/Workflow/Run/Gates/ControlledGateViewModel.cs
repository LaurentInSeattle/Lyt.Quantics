namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

using static GateUiConstants; 

public sealed partial class ControlledGateViewModel : CompositeGateViewModel<ControlledGateView>
{
    // This is NOT the actual key for the gate
    // but the key of the base gate this gate is controlling
    private readonly string baseGateKey;
    private readonly GateParameters gateParameters;

    public ControlledGateViewModel(
        GateParameters gateParameters, int stageIndex,
        QubitsIndices qubitsIndices, bool isGhost = false)
        : base(
            new ControlledGate(GateFactory.Produce(gateParameters.BaseGateKey, gateParameters)),
            stageIndex, qubitsIndices, isGhost)
    {
        this.gateParameters = gateParameters.DeepClone();
        this.baseGateKey = this.gateParameters.BaseGateKey;
        Rectangle rectangle = this.CreateConnectingLine();
        this.contentGrid.Children.Add(rectangle);
        this.CreateGate();
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        if (!this.IsGhost)
        {
            this.DragAble = new DragAble(((MainWindow)App.MainWindow).MainWindowCanvas);
            this.DragAble.Attach(this.View);
            this.View.Content = this.contentGrid;
            this.View.InvalidateVisual();
        }
    }

    public override View CreateGhostView()
    {
        var ghostViewModel = new ControlledGateViewModel(
            this.gateParameters, this.StageIndex, this.QubitsIndices, isGhost: true);
        ghostViewModel.CreateViewAndBind();
        var view = ghostViewModel.View;

        // Do NOT use 'this'.contentGrid (already has a parent and will crash Avalonia) 
        view.Content = ghostViewModel.contentGrid;
        view.ZIndex = 999_999;
        view.Opacity = 0.8;
        view.InvalidateVisual();
        return view;
    }

    private void CreateGate()
    {
        var parameters = this.QubitsIndices;
        var first = this.CreateControlDot();
        int targetIndex = parameters.ControlQuBitIndices[0];
        this.PlaceGridAt(first, targetIndex);

        Grid CreateBaseGate()
        {
            var gate = GateFactory.Produce(this.baseGateKey, this.gateParameters);
            var vm = new GateViewModel(gate, isToolbox: true, isGhost: true);
            vm.CreateViewAndBind();
            var grid = new Grid() { Height = GateSize, Width = GateSize };
            grid.Children.Add(vm.View);
            return grid;
        }

        var last = CreateBaseGate();
        targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(last, targetIndex);
    }
}
