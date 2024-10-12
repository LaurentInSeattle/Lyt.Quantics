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

    public override string Name => "Controlled Controlled Z";

    public override string AlternateName => "CCZ";

    public override string CaptionKey => "CCZ";

    public override GateCategory Category => GateCategory.F_TernaryControlled;
}
