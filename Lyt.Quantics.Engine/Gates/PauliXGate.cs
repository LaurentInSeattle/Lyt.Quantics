namespace Lyt.Quantics.Engine.Gates;

public sealed class PauliXGate : Gate
{
    // The single-qubit Pauli-X gate (σx​).
    // Equivalent to a π radian rotation about the X axis.
    // The gate is equivalent to a classical bit flip.

    private static readonly Complex[,] PauliXGateMatrix = new Complex[,]
    {
        { 0, 1 },
        { 1, 0 }
    };

    public override Complex[,] Matrix => PauliXGate.PauliXGateMatrix;

    public override string Name => "Pauli X";

    public override string AlternateName => "Negate";

    public override string Caption => "X";
}
