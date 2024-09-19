namespace Lyt.Quantics.Engine.Gates;

public sealed class IdentityGate : UnaryGate
{
    private static readonly Complex[,] IdentityGateMatrix = new Complex[,]
    {
        { 1, 0 },
        { 0, 1 }
    };

    protected override Complex[,] GetMatrix() => IdentityGate.IdentityGateMatrix;
}
