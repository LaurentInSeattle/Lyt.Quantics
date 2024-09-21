namespace Lyt.Quantics.Engine.Gates;

using static MathUtilities; 

public sealed class HadamardGate : Gate
{
    // Single-qubit Hadamard gate.
    // This gate is a pi rotation about the X+Z axis, and has the effect of changing computation basis
    // from ∣0⟩,∣1⟩∣0⟩,∣1⟩ to ∣+⟩,∣−⟩∣+⟩,∣−⟩ and vice-versa.
    //
    // In both cases of qubit |0> or qubit |1>, applying a Hadamard Gate gives an equal chance
    // for the qubit to be 0 or 1' when measured.

    private static readonly Complex[,] HadamardMatrix = new Complex[,]
    {
        { 1 / SqrtOfTwo,  1 / SqrtOfTwo },
        { 1 / SqrtOfTwo, -1 / SqrtOfTwo }
    };

    public override Complex[,] Matrix => HadamardGate.HadamardMatrix;

    public override string Name => "Hadamard";

    public override string AlternateName => "Hadamard";

    public override string Caption => "H";
}
