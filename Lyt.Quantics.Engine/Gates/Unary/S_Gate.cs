namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;

#pragma warning disable IDE1006 // Naming Styles
public sealed class S_Gate : Gate
#pragma warning restore IDE1006 
{
    // Single-qubit rotation about the Z axis.
    // This is a diagonal gate. 
    // Special case of the phase gate with phase angle equal to π / 2

    private static readonly Matrix<Complex> S_GateMatrix ;

    static S_Gate()
    {
        //  { 1,  0 },
        //  { 0,  Complex.ImaginaryOne}
        S_GateMatrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);
        S_GateMatrix.At(0, 0, Complex.One);
        S_GateMatrix.At(1, 1, Complex.ImaginaryOne);
    }

    public override Matrix<Complex> Matrix => S_Gate.S_GateMatrix;

    public override string Name => "Phase";

    public override string AlternateName => "Z 90";

    public override string CaptionKey => "S";
}
