namespace Lyt.Quantics.Engine.Machine;

using MathNet.Numerics.LinearAlgebra;

public sealed class QuStage
{
    public QuStage() { /* Required for deserialization */ }

    public List<QuStageOperator> Operators { get; set; } = [];

    [JsonIgnore]
    public QuRegister StageRegister { get; private set; } = new QuRegister(1);

    [JsonIgnore]
    public Matrix<Complex> StageMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1, 1);

    public string Operations
    {
        get
        {
            var sb = new StringBuilder();
            foreach (var op in this.Operators)
            {
                sb.Append(op.GateKey);
                sb.Append("   ");
            }
            return sb.ToString();
        }
    }

    public int ClearAtQubit(int qubitIndex)
    {
        var listToRemove = new List<QuStageOperator>();
        foreach (var stageOperator in this.Operators)
        {
            if (stageOperator.QuBitIndices.Contains(qubitIndex))
            {
                listToRemove.Add(stageOperator);
            }
        }

        foreach (var stageOperator in listToRemove)
        {
            this.Operators.Remove(stageOperator);
        }

        return listToRemove.Count;
    }

    public void AddAtQubit(int qubitIndex, Gate gate)
    {
        var stageOperator = new QuStageOperator() { GateKey = gate.CaptionKey };
        stageOperator.QuBitIndices.Add(qubitIndex);
        this.Operators.Add(stageOperator);
    }

    public bool Validate(QuComputer computer, out string message)
    {
        message = string.Empty;

        foreach (QuStageOperator stageOperator in this.Operators)
        {
            if (!stageOperator.Validate(computer, out message))
            {
                return false;
            }
        }

        return true;
    }

    public bool Build(QuComputer computer, out string message)
    {
        message = string.Empty;

        try
        {
            int length = computer.QuBitsCount;

            // Check for duplicate qubit slots and for empty qubit slots 
            var usedIndices = new HashSet<int>(length);
            var emptyIndices = new HashSet<int>(length);
            for (int i = 0; i < length; i++)
            {
                _ = emptyIndices.Add(i);
            }

            foreach (var stageOperator in this.Operators)
            {
                foreach (int index in stageOperator.QuBitIndices)
                {
                    _ = emptyIndices.Remove(index);
                    if (!usedIndices.Add(index))
                    {
                        // Failed to add: Duplicate index 
                        message =
                            string.Format(
                                "QuBit used in multiple operators, Gate: {0}, QuBit Index: {1}",
                                stageOperator.GateKey, index);
                        return false;
                    }
                }
            }

            // For empty qubit slots, substitute the Identity Gate
            foreach (int index in emptyIndices)
            {
                var quStageOperator =
                    new QuStageOperator() { GateKey = IdentityGate.Key, QuBitIndices = [index] };
                this.Operators.Add(quStageOperator);
            }

            // Reorder the operators by increasing value of their first qubit index 
            var sorted =
                (from op in this.Operators orderby op.QuBitIndices[0] ascending select op)
                .ToList();
            this.Operators = sorted;

            // Build operators 
            foreach (var stageOperator in this.Operators)
            {
                if (!stageOperator.Build(computer, out message))
                {
                    // Failed to build 
                    return false;
                }
            }

            // Combine all operator matrices to create the stage matrix using the Knonecker product
            int powTwo = MathUtilities.TwoPower(length);
            var stageMatrix = this.Operators[0].StageOperatorMatrix;
            if (powTwo == stageMatrix.RowCount)
            {
                this.StageMatrix = stageMatrix;
            }
            else
            {
                for (int i = 1; i < this.Operators.Count; ++i) // Must start at ONE!
                {
                    stageMatrix = stageMatrix.KroneckerProduct(this.Operators[i].StageOperatorMatrix);
                }

                this.StageMatrix = stageMatrix;
            }

            int dimension = this.StageMatrix.RowCount;
            Debug.WriteLine(this.StageMatrix);
            var dagger = this.StageMatrix.ConjugateTranspose();
            var shouldBeIdentity = this.StageMatrix.Multiply(dagger);
            var trueIdentity = Matrix<Complex>.Build.DenseIdentity(dimension, dimension);
            double tolerance = MathUtilities.Epsilon;
            if (!shouldBeIdentity.AlmostEqual(trueIdentity, tolerance))
            {
                message = string.Concat("Stage matrix is not unitary.");
                Debug.WriteLine("shouldBeIdentity: " + shouldBeIdentity);
                Debug.WriteLine("trueIdentity: " + trueIdentity);
                return false;
            }

            // Finally create a register for future calculations 
            this.StageRegister = new QuRegister(length);
        }
        catch (Exception ex)
        {
            message = string.Concat("Exception thrown: " + ex.Message);
            return false;
        }

        return true;
    }

    public bool Calculate(QuRegister sourceRegister, out string message)
    {
        message = string.Empty;
        try
        {
            // Single Step
            this.StageRegister.State = this.StageMatrix.Multiply(sourceRegister.State);
            Debug.WriteLine("Step Result: " + this.StageRegister.State.ToString());
        }
        catch (Exception ex)
        {
            message = string.Concat("Step: Exception thrown: " + ex.Message);
            return false;
        }

        return true;
    }

    public Vector<double> Probabilities =>
        Vector<double>.Build.Dense(this.StageRegister.KetProbabilities().ToArray());
}