namespace Lyt.Quantics.Engine.Gates;

using MathNet.Numerics.LinearAlgebra;

public sealed class ControlledNotGate : Gate
{
    private static readonly Matrix<Complex> ControlledNotMatrix; 

    static ControlledNotGate()
    {
        ControlledNotMatrix = Matrix<Complex>.Build.Sparse(4, 4, Complex.Zero);
        //    { 1, 0, 0, 0 },
        //    { 0, 1, 0, 0 },
        //    { 0, 0, 0, 1 },
        //    { 0, 0, 1, 0 }
        ControlledNotMatrix.At(0, 0, Complex.One);
        ControlledNotMatrix.At(1, 1, Complex.One);
        ControlledNotMatrix.At(2, 3, Complex.One);
        ControlledNotMatrix.At(3, 2, Complex.One);
    }

    public override Matrix<Complex> Matrix => ControlledNotGate.ControlledNotMatrix;

    public override string Name => "Controlled Not";

    public override string AlternateName => "CNOT";

    public override string CaptionKey => "CX";
}
