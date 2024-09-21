namespace Lyt.Quantics.Engine.Utilities; 

public static class MathUtilities
{
    public const double SqrtOfTwo = 1.414_213_562_373_095_048_801_688_724_209_698_078_569_672;
    
    public static int IntegerLog2(int length) => (int)Math.Round(Math.Log(length, 2));

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
    public static Complex[,] Dagger (Complex[,] matrix)
    {
        int matrixDim0 = matrix.GetLength(0);
        int matrixDim1 = matrix.GetLength(1);
        var resultMatrix = new Complex[matrixDim1, matrixDim0 ];
        for (int i = 0; i < matrixDim0; i++)
        {
            for (int j = 0; j < matrixDim1; j++)
            {
                resultMatrix [j,i] = Complex.Conjugate(matrix[i, j]);
            }
        }

        return resultMatrix;
    }
}
