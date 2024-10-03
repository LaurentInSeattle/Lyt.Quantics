namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;

public sealed class PhaseGate : Gate
{
    // Single-qubit rotation about the Z axis.
    // This is a diagonal gate.

    private static readonly Matrix<Complex> PhaseGateMatrix ;

    static PhaseGate()
    {
        //  { 1,  0 },
        //  { 0,  Complex.ImaginaryOne}
        PhaseGateMatrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);
        PhaseGateMatrix.At(0, 0, Complex.One);
        PhaseGateMatrix.At(1, 1, Complex.ImaginaryOne);
    }

    public override Matrix<Complex> Matrix => PhaseGate.PhaseGateMatrix;

    public override string Name => "Phase";

    public override string AlternateName => "Z 90";

    public override string CaptionKey => "S";
}
