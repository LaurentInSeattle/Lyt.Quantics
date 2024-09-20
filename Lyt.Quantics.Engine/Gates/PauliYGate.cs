namespace Lyt.Quantics.Engine.Gates;

public sealed class PauliYGate : UnaryGate
{
    // The single-qubit Pauli-Y gate (σy​).
    // Equivalent to a π radian rotation about the Y axis.
    // The gate is equivalent to a bit and phase flip.

    private static readonly Complex[,] PauliYGateMatrix = new Complex[,]
    {
        { 0,                    -Complex.ImaginaryOne },
        { Complex.ImaginaryOne,  0 }
    };

    public override Complex[,] Matrix => PauliYGate.PauliYGateMatrix;

    public override string Name => "Pauli Y";

    public override string AlternateName => "Y Gate";

    public override string Caption => "Y";
}