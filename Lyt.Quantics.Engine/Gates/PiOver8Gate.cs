namespace Lyt.Quantics.Engine.Gates;

using static MathUtilities;

using MathNet.Numerics.LinearAlgebra;

public sealed class PiOver8Gate : Gate
{
    // Single qubit T gate (Z**0.25).
    // Equivalent to a π/4 radian rotation about the Z axis.
    // It induces a π/4 phase, and is sometimes called the pi/8 gate because of how the RZ(pi/4)
    // matrix looks like.
    // This is a non-Clifford gate and a fourth-root of Pauli-Z.

    // The exponential form of a complex number is in widespread use in engineering and science. 
    // Since z = r(cosθ + isinθ) and since eiθ = cosθ + isinθ we therefore obtain another way in which 
    // to denote a complex number: z = reiθ, called the exponential form.
    // Math.Cos(Math.PI / 4.0 ) == Math.Cos(Math.PI / 4.0 ) == Sqrt ( 2 ) / 2

    private static readonly Matrix<Complex> PiOver8GateMatrix;

    static PiOver8Gate()
    {
        //{ 1, 0 },
        //{ 0, new Complex ( SqrtOfTwo / 2.0 , SqrtOfTwo / 2.0 )}
        var complex = new Complex(SqrtOfTwo / 2.0, SqrtOfTwo / 2.0); 
        PiOver8GateMatrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);
        PiOver8GateMatrix.At(0, 0, Complex.One);
        PiOver8GateMatrix.At(1, 1, complex);
    }

    public override Matrix<Complex> Matrix => PiOver8Gate.PiOver8GateMatrix;

    public override string Name => "T Gate";

    public override string AlternateName => "Pi Over 8";

    public override string CaptionKey => "T";
}