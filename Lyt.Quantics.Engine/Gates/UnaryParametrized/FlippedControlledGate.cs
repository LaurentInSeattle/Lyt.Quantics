namespace Lyt.Quantics.Engine.Gates.UnaryParametrized;

/// <summary> Only used in Unit Test. Keep ! </summary>
public class FlippedControlledGate : Gate
{
    // => See additional comments in ControlledGate
    //
    // The matrix representing the FLIPPED controlled U is: 
    //
    //     CU = [ 1   0     0   0   ] 
    //          [ 0   u00   0   u01 ]
    //          [ 0   0     1   0   ]
    //          [ 0   u10   0   u11 ].

    // Example for Flipped Controlled Hadamard 
    //      SparseMatrix 4x4-Complex 37.50 % Filled
    //
    //      <1; 0>         <0; 0>  <0; 0>          <0; 0>
    //      <0; 0>  <0.707107; 0>  <0; 0>   <0.707107; 0>
    //      <0; 0>         <0; 0>  <1; 0>          <0; 0>
    //      <0; 0>  <0.707107; 0>  <0; 0>  <-0.707107; 0>


    public const string Key = "FCo";

    private readonly Matrix<Complex> matrix;
    // private readonly string baseCaptionKey;

    public FlippedControlledGate(string captionKey) : this(GateFactory.Produce(captionKey)) { }

    /// <summary> Creates a Controlled gate from the provided one (U) via its caption key </summary>
    /// <remarks> Assumes that the Control QuBit is the first one, 'on top' </remarks>
    /// <param name="captionKey"></param>
    public FlippedControlledGate(Gate baseGate)
    {
        if (!baseGate.IsUnary)
        {
            throw new NotSupportedException("Only Controlled Gates based on Unary gates are supported"); 
        }

        this.BaseGate = baseGate;
        int dimension = 2 * baseGate.MatrixDimension;
        this.matrix = Matrix<Complex>.Build.SparseIdentity(dimension);
        var baseMatrix = baseGate.Matrix;
        this.matrix.At(1, 1, baseMatrix.At(0, 0));
        this.matrix.At(1, 3, baseMatrix.At(0, 1));
        this.matrix.At(3, 1, baseMatrix.At(1, 0));
        this.matrix.At(3, 3, baseMatrix.At(1, 1));
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
