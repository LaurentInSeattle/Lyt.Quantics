
namespace Lyt.Quantics.Engine.Gates;

using static MathUtilities; 

public sealed class HadamardGate : UnaryGate
{
    private static readonly Complex[,] HadamardMatrix = new Complex[,]
    {
        { 1 / SqrtOf2,  1 / SqrtOf2 },
        { 1 / SqrtOf2, -1 / SqrtOf2 }
    };

    protected override Complex[,] GetMatrix() => HadamardGate.HadamardMatrix;
}
