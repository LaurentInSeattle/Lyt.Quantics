namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;

public sealed class IdentityGate : Gate
{
    public const string Key = "I"; 

    public static readonly Matrix<Complex> IdentityGateMatrix ;

    static IdentityGate() =>
        //  { 1, 0 },
        //  { 0, 1 }
        IdentityGate.IdentityGateMatrix = Matrix<Complex>.Build.DenseIdentity(2, 2);

    public override Matrix<Complex> Matrix => IdentityGate.IdentityGateMatrix;

    public override string Description => "The Identity Gate";

    public override string Name => "Identity";

    public override string AlternateName => "Identity Gate";

    public override string CaptionKey => IdentityGate.Key;

    public override GateCategory Category => GateCategory.Other;
}
