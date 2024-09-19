namespace Lyt.Quantics.Engine.Gates;

public sealed class PauliZGate : UnaryGate
{
    private static readonly Complex[,] PauliZGateMatrix = new Complex[,]
    {
        { 1,  0 },
        { 0, -1 }
    };

    public override Complex[,] Matrix => PauliZGate.PauliZGateMatrix;
}
