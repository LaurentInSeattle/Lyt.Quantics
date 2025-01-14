namespace Lyt.Quantics.Engine.Gates.Binary;

using MathNet.Numerics.LinearAlgebra;

public sealed class ControlledSGate : Gate
{
    // The CS gate applies a phase change to the target qubit only when the control qubit
    // is in the state |1⟩.
    // If the control qubit is in the state |0⟩, the CS gate does not affect the
    // target qubit. 

    private static readonly Matrix<Complex> ControlledSMatrix; 

    static ControlledSGate()
    {
        ControlledSMatrix = Matrix<Complex>.Build.Sparse(4, 4, Complex.Zero);
        //    { 1, 0, 0,  0 },
        //    { 0, 1, 0,  0 },
        //    { 0, 0, 1,  0 },
        //    { 0, 0, 0,  i }
        ControlledSMatrix.At(0, 0, Complex.One);
        ControlledSMatrix.At(1, 1, Complex.One);
        ControlledSMatrix.At(2, 2, Complex.One);
        ControlledSMatrix.At(3, 3, Complex.ImaginaryOne);
    }

    public override Matrix<Complex> Matrix => ControlledSGate.ControlledSMatrix;

    public override string Description => "The Controlled S Gate, aka Controlled Phase.";

    public override int ControlQuBits => 1;

    public override int TargetQuBits => 1;

    public override string Name => "Controlled S";

    public override string AlternateName => "Controlled Phase S";

    public override string CaptionKey => "CS";

    // Will not show in the toolbox 
    public override GateCategory Category => GateCategory.Special;
}
