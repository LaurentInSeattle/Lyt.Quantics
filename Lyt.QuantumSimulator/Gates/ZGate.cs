namespace Lyt.QuantumSimulator.Gates;

public class ZGate : UnaryGate
{
    protected override Complex[,] GetMatrix() => this.z_matrix;

    private readonly Complex[,] z_matrix = new Complex[,]
    {
        { 1,  0 },
        { 0, -1 }
    };
}
