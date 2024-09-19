namespace Lyt.Quantics.Engine.Gates;

using static MathUtilities; 

public sealed class HadamardGate : UnaryGate
{
    private static readonly Complex[,] HadamardMatrix = new Complex[,]
    {
        { 1 / SqrtOfTwo,  1 / SqrtOfTwo },
        { 1 / SqrtOfTwo, -1 / SqrtOfTwo }
    };

    public override Complex[,] Matrix => HadamardGate.HadamardMatrix;
}
