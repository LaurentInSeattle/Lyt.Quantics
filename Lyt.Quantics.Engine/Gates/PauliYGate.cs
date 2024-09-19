namespace Lyt.Quantics.Engine.Gates;

public sealed class PauliYGate : UnaryGate
{
    private static readonly Complex[,] PauliYGateMatrix = new Complex[,]
    {
        { 0,                    -Complex.ImaginaryOne },
        { Complex.ImaginaryOne,  0 }
    };

    public override Complex[,] Matrix => PauliYGate.PauliYGateMatrix;
}