namespace Lyt.Quantics.Engine.Gates;

public sealed class PauliXGate : UnaryGate
{
    private static readonly Complex[,] PauliXGateMatrix = new Complex[,]
    {
        { 0, 1 },
        { 1, 0 }
    };

    public override Complex[,] Matrix => PauliXGate.PauliXGateMatrix;
}
