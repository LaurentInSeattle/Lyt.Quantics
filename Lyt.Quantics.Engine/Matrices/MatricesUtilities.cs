namespace Lyt.Quantics.Engine.Matrices;

// Cannot use Vector<Complex> using System.Numerics 
// but we still need System.Numerics for Complex 
// Be careful when using global usings 
using MathNet.Numerics.LinearAlgebra;

public static class MatricesUtilities
{
    /// <summary>
    /// Generates the matrix for a swap operation on a register of quBits qubits 
    /// between indices a and b.  
    /// </summary>
    /// <returns> The swap matrix. </returns>
    public static Matrix<Complex> SwapMatrix(int quBits, int a, int b)
    {
        if ((quBits < 0) || (quBits > QuRegister.MaxQubits))
        {
            throw new ArgumentException(null, nameof(quBits));
        }

        if ((a < 0) || (a >= quBits))
        {
            throw new ArgumentException(null, nameof(a));
        }

        if ((b < 0) || (b >= quBits))
        {
            throw new ArgumentException(null, nameof(b));
        }

        if ((a >= b))
        {
            throw new ArgumentException(null, nameof(a));
        }

        if (quBits == 2)
        {
            return new SwapGate().Matrix;
        }

        int delta = b - a;
        if (delta == 1)
        {
            return SingleStageSwapMatrix(quBits, a);
        }

        int stages = 2 * ( delta - 1 ) + 1;
        var reserveSpace = Matrix<Complex>.Build.Dense(1, 1);
        List<Matrix<Complex>> stageMatrices = new(stages);
        for (int i = 0; i < stages; i++)
        {
            stageMatrices.Add(reserveSpace);
        }

        for (int i = 0; i < delta - 1; ++i)
        {
            var swap = SingleStageSwapMatrix(quBits, a + i);
            stageMatrices[i] = swap;
            stageMatrices[stages - 1 - i] = swap;
        }

        var lastSwap = SingleStageSwapMatrix(quBits, b-1);
        stageMatrices[delta-1] = lastSwap;

        var product = stageMatrices[0];
        for (int i = 1; i < stages; ++i)
        {
            product.Multiply(stageMatrices[i]);
        }

        return product;
    }

    /// <summary>
    /// Generates the matrix for a swap operation on a register of quBits qubits 
    /// between indices a and a + 1.  
    /// </summary>
    public static Matrix<Complex> SingleStageSwapMatrix(int quBits, int a)
    {
        List<Matrix<Complex>> matrices = new(quBits);
        for (int i = 0; i < a; i++)
        {
            matrices.Add(new IdentityGate().Matrix);
        }

        // The swap "counts for two", note: i = a + 2 
        matrices.Add(new SwapGate().Matrix);

        for (int i = a + 2; i < quBits; i++)
        {
            matrices.Add(new IdentityGate().Matrix);
        }

        // - Loop starts at one.
        // - note: i < quBits - 1 , again because swap is double dimension
        var kroneckerProduct = matrices[0];
        for (int i = 1; i < quBits - 1; i++)
        {
            kroneckerProduct = kroneckerProduct.KroneckerProduct(matrices[i]);
        }

        return kroneckerProduct;
    }

    [Conditional("DEBUG")]
    public static void VerifyMatrix (Matrix<Complex> matrix)
    {
        // Debug.WriteLine(matrix);
        int dimension = matrix.RowCount;
        var dagger = matrix.ConjugateTranspose();
        var shouldBeIdentity = matrix.Multiply(dagger);
        var trueIdentity = Matrix<Complex>.Build.DenseIdentity(dimension, dimension);
        double tolerance = MathUtilities.Epsilon;
        if (!shouldBeIdentity.AlmostEqual(trueIdentity, tolerance))
        {            
            Debug.WriteLine("Matrix is not unitary.");
            Debug.WriteLine(matrix);
            Debug.WriteLine("shouldBeIdentity: " + shouldBeIdentity);
            Debug.WriteLine("trueIdentity: " + trueIdentity);
            if( Debugger.IsAttached ) { Debugger.Break(); }
        }
    }
}
