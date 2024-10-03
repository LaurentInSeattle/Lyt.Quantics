namespace Lyt.Quantics.Engine.Gates.Binary;

using MathNet.Numerics.LinearAlgebra;

public sealed class SwapGate : Gate
{
    // The swap gate swaps two qubits. 
        
    private static readonly Matrix<Complex> SwapMatrix;

    static SwapGate()
    {
        //    { 1, 0, 0, 0 },
        //    { 0, 0, 1, 0 },
        //    { 0, 1, 0, 0 },
        //    { 0, 0, 0, 1 },
        SwapMatrix = Matrix<Complex>.Build.Sparse(4, 4, Complex.Zero);
        SwapMatrix.At(0, 0, Complex.One);
        SwapMatrix.At(3, 3, Complex.One);
        SwapMatrix.At(1, 2, Complex.One);
        SwapMatrix.At(2, 1, Complex.One);
    }

    public override Matrix<Complex> Matrix => SwapGate.SwapMatrix;

    public override string Name => "Swap";

    public override string AlternateName => "FCNOT";

    public override string CaptionKey => "Swap";
}
