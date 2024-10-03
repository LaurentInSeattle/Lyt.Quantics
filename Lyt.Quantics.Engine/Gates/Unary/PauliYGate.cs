namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;

public sealed class PauliYGate : Gate
{
    // The single-qubit Pauli-Y gate (σy​).
    // Equivalent to a π radian rotation about the Y axis.
    // The gate is equivalent to a bit and phase flip.

    private static readonly Matrix<Complex> PauliYGateMatrix ;

    static PauliYGate()
    {
        //        { 0,                    -Complex.ImaginaryOne },
       //         { Complex.ImaginaryOne,  0 }
        PauliYGateMatrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);
        PauliYGateMatrix.At(0, 1, -Complex.ImaginaryOne);
        PauliYGateMatrix.At(1, 0, Complex.ImaginaryOne);
    }

    public override Matrix<Complex> Matrix => PauliYGate.PauliYGateMatrix;

    public override string Name => "Pauli Y";

    public override string AlternateName => "Y Gate";

    public override string CaptionKey => "Y";
}