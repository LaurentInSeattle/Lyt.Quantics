namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;
using static MathUtilities;

public sealed class HadamardGate : Gate
{
    // Single-qubit Hadamard gate.
    // This gate is a pi rotation about the X+Z axis, and has the effect of changing computation basis
    // from ∣0⟩,∣1⟩∣0⟩,∣1⟩ to ∣+⟩,∣−⟩∣+⟩,∣−⟩ and vice-versa.
    //
    // In both cases of qubit |0> or qubit |1>, applying a Hadamard Gate gives an equal chance
    // for the qubit to be 0 or 1 when measured.

    public const string Key = "H"; 

    private static readonly Matrix<Complex> HadamardGateMatrix; 

    static HadamardGate()
    {
        // { 1 / SqrtOfTwo,  1 / SqrtOfTwo },
        // { 1 / SqrtOfTwo, -1 / SqrtOfTwo }
        var matrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.One);
        matrix.At(1, 1, -Complex.One);
        HadamardGateMatrix = matrix.Divide(SqrtOfTwo);
    }

    public override Matrix<Complex> Matrix => HadamardGate.HadamardGateMatrix;

    public override string Description => "The Hadamard Gate";

    public override string Name => "Hadamard";

    public override string AlternateName => "Hadamard";

    public override string CaptionKey { get; set; } = Key;

    public override GateCategory Category => GateCategory.A_HadamardAndT;
}
