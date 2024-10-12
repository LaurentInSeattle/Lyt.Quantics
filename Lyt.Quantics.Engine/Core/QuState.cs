namespace Lyt.Quantics.Engine.Core;

using static MathUtilities;

using MathNet.Numerics.LinearAlgebra;

public enum QuState
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
    public static Complex[] ToTensor(this QuState state)
        => state switch
        {
            QuState.One => [Complex.Zero, Complex.One],
            QuState.Plus => [Complex.One / SqrtOfTwo, Complex.One / SqrtOfTwo],
            QuState.Minus => [Complex.One / SqrtOfTwo, -Complex.One / SqrtOfTwo],
            QuState.PlusIota => [Complex.One / SqrtOfTwo, Complex.ImaginaryOne / SqrtOfTwo],
            QuState.MinusIota => [Complex.One / SqrtOfTwo, -Complex.ImaginaryOne / SqrtOfTwo],
            _ => [Complex.One, Complex.Zero], // Basis state Zero 
        };

    public static Vector<Complex> ToVector(this Complex[] complexes)
        => Vector<Complex>.Build.Dense(complexes);

    public static Vector<Complex> ToVector(this QuState state)
        => Vector<Complex>.Build.Dense(state.ToTensor());

    public static string ToUiString(this QuState state)
        => state switch
        {
            QuState.One => "1",
            QuState.Plus => "+",
            QuState.Minus => "-",
            QuState.PlusIota => "i",
            QuState.MinusIota => "-i",
            _ => "0", // Basis state Zero 
        };
}
