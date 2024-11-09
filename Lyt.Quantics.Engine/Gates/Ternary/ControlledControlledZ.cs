namespace Lyt.Quantics.Engine.Gates.Ternary;

public sealed class ControlledControlledZ : Gate
{
    private static readonly Matrix<Complex> CCzMatrix;

    static ControlledControlledZ()
    {
        var gate = new ControlledGate(new ControlledZGate());
        ControlledControlledZ.CCzMatrix = gate.Matrix;
    }

    public override Matrix<Complex> Matrix => ControlledControlledZ.CCzMatrix;

    // NOT SURE...  Figure that out 
    // Can we say that:
    //      The three Qubits are equivalent and hence should all be considered as Targets ??? 
    public override int ControlQuBits => 0;

    public override int TargetQuBits => 3;

    // END  ~  NOT SURE...  Figure that out 

    public override string Name => "Controlled Controlled Z";

    public override string AlternateName => "CCZ";

    public override string CaptionKey => "CCZ";

    public override GateCategory Category => GateCategory.G_TernaryControlled;
}
