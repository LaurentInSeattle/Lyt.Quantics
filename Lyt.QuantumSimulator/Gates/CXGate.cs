namespace Lyt.QuantumSimulator.Gates;

public class CXGate : BinaryGate
{
    protected override Complex[,] GetMatrix() => this.cx_matrix;

    private readonly Complex[,] cx_matrix = new Complex[,]
    {
        { 1, 0, 0, 0 },
        { 0, 1, 0, 0 },
        { 0, 0, 0, 1 },
        { 0, 0, 1, 0 }
    };
}
