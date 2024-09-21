namespace Lyt.Quantics.Engine.Gates;

public sealed class PhaseGate : Gate
{
    // Single-qubit rotation about the Z axis.
    // This is a diagonal gate.

    private static readonly Complex[,] PhaseGateMatrix = new Complex[,]
    {
        { 1,  0 },
        { 0,  Complex.ImaginaryOne}
    };

    public override Complex[,] Matrix => PhaseGate.PhaseGateMatrix;

    public override string Name => "Phase";

    public override string AlternateName => "Z 90";

    public override string Caption => "S";
}
