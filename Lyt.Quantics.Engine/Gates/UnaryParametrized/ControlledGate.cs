namespace Lyt.Quantics.Engine.Gates.UnaryParametrized;

public class ControlledGate : Gate
{
    // Controlled gates act on 2 or more qubits, where one or more qubits act as a control for
    // some operation. For example, the controlled NOT gate (or CNOT or CX) acts on 2 qubits,
    // and performs the NOT operation on the second qubit only when the first qubit is | 1 ⟩
    // and otherwise leaves it unchanged. 
    // More generally if U is a gate that operates on a single qubit, then the controlled-U
    // gate is a gate that operates on two qubits in such a way that the first qubit serves
    // as a control. It maps the basis states as follows.
    //
    //  | 00 ⟩ ↦ | 00 ⟩
    //  | 01 ⟩ ↦ | 01 ⟩
    //  | 10 ⟩ ↦ | 1 ⟩ ⊗ U | 0 ⟩ = | 1 ⟩ ⊗ (u 00 | 0 ⟩ + u 10 | 1 ⟩ ) 
    //  | 11 ⟩ ↦ | 1 ⟩ ⊗ U | 1 ⟩ = | 1 ⟩ ⊗ (u 01 | 0 ⟩ +u 11 | 1 ⟩ ) 
    // 
    // Therefore the matrix representing the controlled U is: 
    //
    //     CU = [ 1   0   0   0   ] 
    //          [ 0   1   0   0   ]
    //          [ 0   0   u00 u01 ]
    //          [ 0   0   u10 u11 ].

    public const string Key = "Co";

    private readonly Matrix<Complex> matrix;
    // private readonly string baseCaptionKey;

    public ControlledGate(string captionKey) : this(GateFactory.Produce(captionKey)) { }

    /// <summary> Creates a Controlled gate from the provided one (U) via its caption key </summary>
    /// <remarks> Assumes that the Control QuBit is the first one, 'on top' </remarks>
    /// <param name="captionKey"></param>
    public ControlledGate(Gate baseGate)
    {
        // DONT do that: This code is also used to create CCX, CSWAP and CCZ
        //if (!baseGate.IsUnary)
        //{
        //    throw new NotSupportedException("Only Controlled Gates based on Unary gates are supported");
        //}

        if (baseGate.IsTernary)
        {
            throw new NotSupportedException("Quaternary gates are not supported");
        }

        this.BaseGate = baseGate;
        int baseDimension = baseGate.MatrixDimension;
        int dimension = 2 * baseDimension;
        int delta = dimension - baseDimension;
        this.matrix = Matrix<Complex>.Build.Sparse(dimension, dimension, Complex.Zero);

        // Identity part 
        for (int diagonal = 0; diagonal < delta; ++diagonal)
        {
            this.matrix.At(diagonal, diagonal, Complex.One);
        }

        // Copy values from base into bottom right square area
        // Indices then start at position delta
        var baseMatrix = baseGate.Matrix;
        for (int row = delta; row < dimension; ++row)
        {
            for (int col = delta; col < dimension; ++col)
            {
                int baseRow = row - delta;
                int baseCol = col - delta;
                this.matrix.At(row, col, baseMatrix.At(baseRow, baseCol));
            }
        }
    }

    public Gate BaseGate { get; private set; }

    public override int ControlQuBits => 1;

    public override int TargetQuBits => 1;

    public override Matrix<Complex> Matrix => this.matrix;

    public override string Description => "Controlled " + this.BaseGate.Description;

    public override string Name => "Controlled " + this.BaseGate.Name;

    public override string AlternateName => this.Name;

    public override string CaptionKey => "Co";

    public override GateCategory Category => GateCategory.Special;
}
