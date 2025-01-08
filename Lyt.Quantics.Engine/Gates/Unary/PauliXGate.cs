namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;

public sealed class PauliXGate : Gate
{
    // The single-qubit Pauli-X gate (σx​).
    // Equivalent to a π radian rotation about the X axis.
    // The gate is equivalent to a classical bit flip.

    private static readonly Matrix<Complex> PauliXGateMatrix;

    public const string Key = "X";

    static PauliXGate()
    {
        //  { 0, 1 },
        //  { 1, 0 }
        PauliXGateMatrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.One);
        PauliXGateMatrix.At(0, 0, Complex.Zero);
        PauliXGateMatrix.At(1, 1, Complex.Zero);
    }

    public override Matrix<Complex> Matrix => PauliXGate.PauliXGateMatrix;

    public override string Description => "The Pauli X Gate, aka Negate";

    public override string Name => "Pauli X";

    public override string AlternateName => "Negate";

    public override string CaptionKey { get; set; } = Key;

    public override GateCategory Category => GateCategory.B_Pauli;
}
