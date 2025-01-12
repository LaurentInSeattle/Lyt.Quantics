namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class ControlledGateViewModel : GateViewModelBase<ControlledGateView>
{
    public ControlledGateViewModel(
        string gateKey, int stageIndex, 
        QubitsIndices qubitsIndices, bool isGhost = false)
        : base(
            GateFactory.Produce(gateKey, new GateParameters()),
            qubitsIndices, isGhost, isToolBox: false, stageIndex)
    {
    } 

    public override UserControl CreateGhostView()
    {
        return new();
    }

    public double GateHeight { get => this.Get<double>(); set => this.Set(value); }
}
