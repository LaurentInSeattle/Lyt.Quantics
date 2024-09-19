namespace Lyt.Quantics.Engine.Utilities; 

public static class MathUtilities
{
    public static double SqrtOf2 = Math.Sqrt(2);
    
    public static int Log2(int length) => (int)Math.Round(Math.Log(length, 2));

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
}
