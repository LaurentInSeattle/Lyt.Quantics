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

    [JsonIgnore]
    public Vector<double> KetProbabilities =>
        Vector<double>.Build.Dense([.. this.StageRegister.KetProbabilities()]);

    [JsonIgnore]
    public Vector<double> QuBitProbabilities =>
        Vector<double>.Build.Dense([.. this.StageRegister.QuBitProbabilities()]);

    [JsonIgnore]
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

    /// <returns> True if the only operators are Identity. </returns>
    public bool IsEmpty()
    {
        foreach (var op in this.Operators)
        {
            if (op.GateKey != IdentityGate.Key)
            {
                return false;
            }
        }

        return true;
    }

    public QuStageOperator StageOperatorAt(QubitsIndices qubitsIndices)
    {
        foreach (int qubitIndex in qubitsIndices.AllQubitIndicesSorted())
        {
            foreach (var stageOperator in Operators)
            {
                if (stageOperator.TargetQuBitIndices.Count > 0)
                {
                    if (stageOperator.TargetQuBitIndices[0] == qubitIndex)
                    {
                        return stageOperator;
                    }
                }

                if (stageOperator.ControlQuBitIndices.Count > 0)
                {
                    if (stageOperator.ControlQuBitIndices[0] == qubitIndex)
                    {
                        return stageOperator;
                    }
                }
            }
        }

        throw new Exception("Failed to retrieve Stage Operator");
    }

    public QuStage DeepClone()
    {
        var clone = new QuStage();
        foreach (var stageOperator in this.Operators)
        {
            clone.Operators.Add(stageOperator.DeepClone());
        }

        return clone;
    }

    public int ClearAtQubit(int qubitIndex)
    {
        var listToRemove = new List<QuStageOperator>();
        foreach (var stageOperator in this.Operators)
        {
            if ((stageOperator.TargetQuBitIndices.Contains(qubitIndex)) ||
                (stageOperator.ControlQuBitIndices.Contains(qubitIndex)))
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

    /// <summary> If isDrop is true we only have one target qubit index ! </summary>
    public void AddAtQubit(QuComputer computer, QubitsIndices qubitsIndices, Gate gate, bool isDrop)
    {
        var stageOperator = new QuStageOperator(gate, qubitsIndices, isDrop);
        if (!stageOperator.Validate(computer, out string message))
        {
            throw new Exception(message);
        }

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

        // There should not be any overlapping qubit indices in the operators of the stage 
        HashSet<int> set = new(computer.QuBitsCount);
        foreach (QuStageOperator stageOperator in this.Operators)
        {
            foreach (int qubitIndex in stageOperator.ControlQuBitIndices)
            {
                bool added = set.Add(qubitIndex);
                if (!added)
                {
                    message = stageOperator.GateKey + ": Overlapping Control qubit indices";
                    return false;
                }
            }

            foreach (int qubitIndex in stageOperator.TargetQuBitIndices)
            {
                bool added = set.Add(qubitIndex);
                if (!added)
                {
                    message = stageOperator.GateKey + ": Overlapping Target qubit index: " + qubitIndex.ToString();
                    return false;
                }
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
                foreach (int index in stageOperator.AllQuBitIndicesSorted)
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
                    new QuStageOperator(IdentityGate.Key) { TargetQuBitIndices = [index] };
                this.Operators.Add(quStageOperator);
            }

            // Reorder the operators by increasing value of their first qubit index 
            var sorted =
                (from op in this.Operators orderby op.SmallestQubitIndex ascending select op)
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
            // Debug.WriteLine(this.StageMatrix);
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
            if (this.IsEmpty())
            {
                this.StageRegister.State = sourceRegister.State.Clone();
            }
            else
            {
                this.StageRegister.State = this.StageMatrix.Multiply(sourceRegister.State);
                // Debug.WriteLine("Step Result: " + this.StageRegister.State.ToString());
            }
        }
        catch (Exception ex)
        {
            message = string.Concat("Step: Exception thrown: " + ex.Message);
            return false;
        }

        return true;
    }
}