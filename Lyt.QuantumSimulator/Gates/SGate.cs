namespace Lyt.QuantumSimulator.Gates;

public class SGate : UnaryGate
{
    // TODO: Tests 

    protected override Complex[,] GetMatrix() => this.s_matrix;

    private readonly Complex[,] s_matrix = new Complex[,]
    {
        { Complex.One , Complex.Zero },
        { Complex.Zero, Complex.ImaginaryOne }
    };
}
