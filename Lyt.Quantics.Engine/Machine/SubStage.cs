namespace Lyt.Quantics.Engine.Machine;

using Lyt.Quantics.Engine.Matrices;

public sealed class SubStage(QuStageOperator stageOperator)
{
    public readonly QuStageOperator StageOperator = stageOperator;

    public Matrix<Complex> SubStageMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1, 1);

    public bool Build(QuComputer computer, out string message)
    {
        try
        {
            message = string.Empty;
            int length = computer.QuBitsCount;
            int dimension = MathUtilities.IntegerLog2(this.StageOperator.StageOperatorMatrix.RowCount);
            var identity = Matrix<Complex>.Build.DenseIdentity(2, 2);

            Matrix<Complex> SelectMatrix(int index)
            {
                if (index == this.StageOperator.SmallestQubitIndex)
                {
                    return this.StageOperator.StageOperatorMatrix;
                }
                else
                {
                    return identity;
                }
            }

            // Combine this operator matrix with identity matrices to create the sub-stage
            // matrix using the Knonecker product.
            var stageMatrix = SelectMatrix(0);
            int step = dimension - 1;
            int startIndex = 1;
            if (0 == this.StageOperator.SmallestQubitIndex)
            {
                startIndex += step;
            }

            for (int i = startIndex; i < length; ++i) // Must start at at least ONE!
            {
                var currentMatrix = SelectMatrix(i);
                stageMatrix = stageMatrix.KroneckerProduct(currentMatrix);

                if (i == this.StageOperator.SmallestQubitIndex)
                {
                    i += step;
                }
            }

            this.SubStageMatrix = stageMatrix;
            MatricesUtilities.VerifyMatrix(stageMatrix);
        }
        catch (Exception ex)
        {
            message = string.Concat("Exception thrown: " + ex.Message);
            return false;
        }

        return true;
    }
}
