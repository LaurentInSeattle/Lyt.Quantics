namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;

public sealed class IdentityGate : Gate
{
    public const string Key = "I"; 

    private static readonly Matrix<Complex> IdentityGateMatrix ; 

    static IdentityGate()
    {
        //  { 1, 0 },
        //  { 0, 1 }
        IdentityGateMatrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.One);
        IdentityGateMatrix.At(1, 0, Complex.Zero);
        IdentityGateMatrix.At(0, 1, Complex.Zero);
    }

    public override Matrix<Complex> Matrix => IdentityGate.IdentityGateMatrix;

    public override string Name => "Identity";

    public override string AlternateName => "Identity Gate";

    public override string CaptionKey => IdentityGate.Key;

    public override GateCategory Category => GateCategory.F_Other;
}
