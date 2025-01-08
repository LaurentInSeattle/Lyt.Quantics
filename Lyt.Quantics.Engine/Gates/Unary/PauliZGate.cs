namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;

public sealed class PauliZGate : Gate
{
    // The single-qubit Pauli-Z gate (σz).
    // Equivalent to a π radian rotation about the Z axis.
    // The gate is equivalent to a phase flip.

    private static readonly Matrix<Complex> PauliZGateMatrix;

    static PauliZGate()
    {
        //  { 1,  0 },
        //  { 0, -1 }
        PauliZGateMatrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);
        PauliZGateMatrix.At(0, 0, Complex.One);
        PauliZGateMatrix.At(1, 1, -Complex.One);
    }

    public override Matrix<Complex> Matrix => PauliZGate.PauliZGateMatrix;

    public override string Description => "The Pauli Z Gate, aka Phase Flip";

    public override string Name => "Pauli Z";

    public override string AlternateName => "Z Gate";

    public override string CaptionKey => "Z";

    public override GateCategory Category => GateCategory.B_Pauli;
}