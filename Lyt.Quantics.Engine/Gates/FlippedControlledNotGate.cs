namespace Lyt.Quantics.Engine.Gates;

using MathNet.Numerics.LinearAlgebra;

public sealed class FlippedControlledNotGate : Gate
{
    private static readonly Matrix<Complex> FlippedControlledNotMatrix; 

    static FlippedControlledNotGate()
    {
        //    { 1, 0, 0, 0 },
        //    { 0, 0, 0, 1 },
        //    { 0, 0, 1, 0 },
        //    { 0, 1, 0, 0 },
        FlippedControlledNotMatrix = Matrix<Complex>.Build.Sparse(4, 4, Complex.Zero);
        FlippedControlledNotMatrix.At(0, 0, Complex.One);
        FlippedControlledNotMatrix.At(2, 2, Complex.One);
        FlippedControlledNotMatrix.At(1, 3, Complex.One);
        FlippedControlledNotMatrix.At(3, 1, Complex.One);
    }

    public override Matrix<Complex> Matrix => FlippedControlledNotGate.FlippedControlledNotMatrix;

    public override string Name => "Flipped Controlled Not";

    public override string AlternateName => "FCNOT";

    public override string CaptionKey => "FCX";
}
