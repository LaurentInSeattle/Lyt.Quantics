namespace Lyt.Quantics.Engine.Gates;

public sealed class IdentityGate : UnaryGate
{
    private static readonly Complex[,] IdentityGateMatrix = new Complex[,]
    {
        { 1, 0 },
        { 0, 1 }
    };

    public override Complex[,] Matrix => IdentityGate.IdentityGateMatrix;

    public override string Name => "Identity";

    public override string AlternateName => "Identity Gate";

    public override string Caption => "I";
}
