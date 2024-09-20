namespace Lyt.Quantics.Engine.Gates;

public sealed class PauliZGate : UnaryGate
{
    // The single-qubit Pauli-Z gate (σz).
    // Equivalent to a π radian rotation about the Z axis.
    // The gate is equivalent to a phase flip.

    private static readonly Complex[,] PauliZGateMatrix = new Complex[,]
    {
        { 1,  0 },
        { 0, -1 }
    };

    public override Complex[,] Matrix => PauliZGate.PauliZGateMatrix;

    public override string Name => "Pauli Z";

    public override string AlternateName => "Z Gate";

    public override string Caption => "Z";
}