namespace Lyt.QuantumSimulator.Gates;

public class CZGate : BinaryGate
{
    protected override Complex[,] GetMatrix() => cz_matrix;

    private readonly Complex[,] cz_matrix = new Complex[,]
    {
        { 1, 0, 0, 0 },
        { 0, 1, 0, 0 },
        { 0, 0, 1, 0 },
        { 0, 0, 0, -1 }
    };
}
