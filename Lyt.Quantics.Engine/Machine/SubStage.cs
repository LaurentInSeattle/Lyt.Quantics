using Lyt.Quantics.Engine.Matrices;
using System;

namespace Lyt.Quantics.Engine.Machine;

public sealed class SubStage(QuStageOperator stageOperator)
{
    private readonly QuStageOperator stageOperator = stageOperator;

    public Matrix<Complex> SubStageMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1, 1);

    public bool Build(QuComputer computer, out string message)
    {
        try
        {
            message = string.Empty;
            int length = computer.QuBitsCount;
            int dimension = MathUtilities.IntegerLog2(this.stageOperator.StageOperatorMatrix.RowCount);
            var identity = Matrix<Complex>.Build.DenseIdentity(2, 2);

            Matrix<Complex> SelectMatrix(int index)
            {
                if (index == this.stageOperator.SmallestQubitIndex)
                {
                    return this.stageOperator.StageOperatorMatrix;
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
            if (0 == this.stageOperator.SmallestQubitIndex)
            {
                startIndex += step;
            }

            for (int i = startIndex; i < length; ++i) // Must start at at least ONE!
            {
                var currentMatrix = SelectMatrix(i);
                stageMatrix = stageMatrix.KroneckerProduct(currentMatrix);

                if (i == this.stageOperator.SmallestQubitIndex)
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
