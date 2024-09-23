namespace Lyt.Quantics.Engine.Core;

using static MathUtilities;

using MathNet.Numerics.LinearAlgebra;

public enum BasisState
{
    Zero,
    One,
    Plus,
    Minus,
    PlusIota,
    MinusIota,
}

public static class BasisStateExtensions
{
    public static Complex[] ToTensor(this BasisState state)
        => state switch
        {
            BasisState.One => [Complex.Zero, Complex.One],
            BasisState.Plus => [Complex.One / SqrtOfTwo, Complex.One / SqrtOfTwo],
            BasisState.Minus => [Complex.One / SqrtOfTwo, -Complex.One / SqrtOfTwo],
            BasisState.PlusIota => [Complex.One / SqrtOfTwo, Complex.ImaginaryOne / SqrtOfTwo],
            BasisState.MinusIota => [Complex.One / SqrtOfTwo, -Complex.ImaginaryOne / SqrtOfTwo],
            _ => [Complex.One, Complex.Zero], // Basis state Zero 
        };

    public static Vector<Complex> ToVector(this Complex[] complexes)
        => Vector<Complex>.Build.Dense(complexes);
}
