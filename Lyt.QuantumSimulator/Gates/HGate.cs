namespace Lyt.QuantumSimulator.Gates;

public class HGate : UnaryGate
{
    protected override Complex[,] GetMatrix() => this.h_matrix;

    private readonly Complex[,] h_matrix = new Complex[,]
    {
        { 1 / Math.Sqrt(2), 1 / Math.Sqrt(2) },
        { 1 / Math.Sqrt(2), -1 / Math.Sqrt(2) }
    };
}
