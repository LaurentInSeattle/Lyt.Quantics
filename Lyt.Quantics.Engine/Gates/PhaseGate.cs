namespace Lyt.Quantics.Engine.Gates;

public sealed class PhaseGate : UnaryGate
{
    private static readonly Complex[,] PhaseGateMatrix = new Complex[,]
    {
        { 1,  0 },
        { 0,  Complex.ImaginaryOne}
    };

    protected override Complex[,] GetMatrix() => PhaseGate.PhaseGateMatrix;
}
