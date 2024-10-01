namespace Lyt.Quantics.Engine.Utilities;

// Cannot use Vector<Complex> using System.Numerics 
// but we still need System.Numerics for Complex 
// Be careful when using global usings 
using MathNet.Numerics.LinearAlgebra;

public static class MathUtilities
{
    public const double Epsilon = 0.000_000_000_000_1;

    public const double SqrtOfTwo = 1.414_213_562_373_095_048_801_688_724_209_698_078_569_672;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int IntegerLog2(int length) => (int)Math.Round(Math.Log(length, 2));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int TwoPower(int power) => 1 << power;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsPowerOfTwo(int n) => n != 0 && (n & (n - 1)) == 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(int x, int pos) => (x & (1 << pos)) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetBit(int x, int pos) => x | (1 << pos);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClearBit(int x, int pos) => x & (~(1 << pos));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetBitValue(int x, int pos, bool v) => v ? (x | (1 << pos)) : (x & (~(1 << pos)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool AreAlmostEqual(double x, double y)
        => Math.Abs(x - y) <= MathUtilities.Epsilon; 

    public static double SquaredMagnitude(this Complex[] tensor)
    {
        double magnitude = 0.0;
        for (int i = 0; i < tensor.Length; ++i)
        {
            Complex complex = tensor[i];
            magnitude += (Complex.Conjugate(complex) * complex).Real;
        }

        return magnitude;
    }

    public static void DivideBy(this Complex[] tensor, double divisor)
    {
        if (Math.Abs(divisor) <= double.Epsilon)
        {
            throw new ArgumentException("Zero divide");
        }

        for (int i = 0; i < tensor.Length; ++i)
        {
            tensor[i] /= divisor;
        }
    }

    public static void MultiplyBy(this Complex[] tensor, double factor)
    {
        for (int i = 0; i < tensor.Length; ++i)
        {
            tensor[i] /= factor;
        }
    }

    /// <summary> Tensor product for two single row matrices using Math.Net. </summary>
    //public static Vector<Complex> TensorProduct(Vector<Complex> v1, Vector<Complex> v2)
    //{
    //    Matrix<Complex> mat1 = Matrix<Complex>.Build.Dense(1, v1.Count, v1.AsArray());
    //    Matrix<Complex> mat2 = Matrix<Complex>.Build.Dense(v2.Count, 1, v2.AsArray());
    //    var kron = mat1.KroneckerProduct(mat2);
    //    var flat = kron.ToColumnMajorArray();
    //    return Vector<Complex>.Build.Dense(flat) ;
    //}

    /// <summary> Tensor product for the (very) special case of two single row matrices. </summary>
    /// <remarks> Similar to Outer Product but returns a flat vector. </remarks>
    /// <remarks> Also known as Kronecker product, operator 'kron' in some libraries. </remarks>
    public static Vector<Complex> TensorProduct(Vector<Complex> v1, Vector<Complex> v2)
    {
        int v1Length = v1.Count;
        int v2Length = v2.Count;
        var resultVector = Vector<Complex>.Build.Dense(v1Length * v2Length);
        for (int i = 0; i < v1Length; ++i)
        {
            for (int j = 0; j < v2Length; ++j)
            {
                resultVector[i * v2Length + j] = v1[i] * v2[j];
            }
        }

        return resultVector;
    }

    /// <summary> Tensor product for the (very) special case of two single row matrices. </summary>
    public static Complex[] TensorProduct(Complex[] v1, Complex[] v2)
    {
        int v1Length = v1.Length;
        int v2Length = v2.Length;
        var resultVector = new Complex[v1Length * v2Length];
        for (int i = 0; i < v1Length; ++i)
        {
            for (int j = 0; j < v2Length; ++j)
            {
                resultVector[i * v2Length + j] = v1[i] * v2[j];
            }
        }

        return resultVector;
    }

    /// <summary> Transform provided vector using the given matrix. </summary>
    /// <exception cref="InvalidOperationException"> if mismatch of dimensions </exception>
    public static Complex[] Transform(Complex[] vector, Complex[,] matrix)
    {
        int matrixDim0 = matrix.GetLength(0);
        if (matrixDim0 != vector.Length)
        {
            throw new InvalidOperationException();
        }

        int matrixDim1 = matrix.GetLength(1);
        var resultVector = new Complex[vector.Length];
        for (int i = 0; i < matrixDim0; i++)
        {
            resultVector[i] = Complex.Zero;
            for (int j = 0; j < matrixDim1; j++)
            {
                resultVector[i] += vector[j] * matrix[i, j];
            }
        }

        return resultVector;
    }

    /// <summary> Calculates and returns the conjugate transpose of the provided matrix. </summary>
    /// <remarks> 
    /// Aka: Hermitian conjugate, adjoint matrix or transjugate. 
    /// See: https://en.wikipedia.org/wiki/Conjugate_transpose 
    /// </remarks>
    public static Complex[,] Dagger(Complex[,] matrix)
    {
        int matrixDim0 = matrix.GetLength(0);
        int matrixDim1 = matrix.GetLength(1);
        var resultMatrix = new Complex[matrixDim1, matrixDim0];
        for (int i = 0; i < matrixDim0; i++)
        {
            for (int j = 0; j < matrixDim1; j++)
            {
                resultMatrix[j, i] = Complex.Conjugate(matrix[i, j]);
            }
        }

        return resultMatrix;
    }
}
