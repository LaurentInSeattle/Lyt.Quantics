namespace Lyt.Quantics.Engine.Machine;

public sealed partial class QuComputer
{
    public bool CreateNew (out string message)
    {
        try
        {
            this.InitialStates.Add(QuState.Zero);
            this.QuBitsCount = this.InitialStates.Count;
            this.KetMap = new KetMap (this.QuBitsCount);
            if (this.Stages.Count == 0)
            {
                QuStage quStage = new();
                this.Stages.Add(quStage);
            }

            // Stages: nothing to do (for now?)  
            this.IsValid = false;
            return this.Validate(out message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Add Qubit: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool AddQubitAtEnd(out string message)
    {
        try
        {
            this.InitialStates.Add(QuState.Zero);
            this.QuBitsCount = this.InitialStates.Count;
            this.KetMap = new KetMap(this.QuBitsCount);
            if (this.Stages.Count == 0)
            {
                QuStage quStage = new();
                this.Stages.Add(quStage);
            }

            // Stages: nothing to do (for now?)  
            this.IsValid = false;
            return this.Validate(out message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Add Qubit: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool RemoveLastQubit(out string message)
    {
        try
        {
            int qubitIndex = this.QuBitsCount - 1;
            this.InitialStates.RemoveAt(qubitIndex);
            this.QuBitsCount = this.InitialStates.Count;
            this.KetMap = new KetMap(this.QuBitsCount);

            // For all stages : Remove all operators 'touching' this qubit index 
            foreach (var stage in this.Stages)
            {
                stage.ClearAtQubit(qubitIndex);
            }

            // Remove all empty stages 
            this.PackStages(out message); 

            this.IsValid = false;
            return this.Validate(out message);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Remove Qubit: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool UpdateQubit(int qubitIndex, QuState newState, out string message)
    {
        try
        {
            if ((qubitIndex < 0) || (qubitIndex >= this.QuBitsCount))
            {
                message = "Invalid Qubit index.";
                return false;
            }

            this.InitialStates[qubitIndex] = newState;
            if (this.IsBuilt)
            {
                // This 'shortcut' becomes important at 6 qubits and above 
                message = string.Empty;
                return true;
            } 

            this.IsValid = false;
            return this.Validate(out message);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Update Qubit: Exception thrown: " + ex.Message;
            return false;
        }
    }

    /// <summary> If isDrop is true we only have one target qubit index ! </summary>
    public bool AddGate(
        int stageIndex, QubitsIndices qubitsIndices, Gate gate, bool isDrop, out string message)
    {
        try
        {
            message = string.Empty;
            // Allow : stageIndex == this.Stages.Count
            if ((stageIndex < 0) || (stageIndex > this.Stages.Count))
            {
                message = "Add Gate: Invalid Stage index.";
                return false;
            }

            var allIndices = qubitsIndices.AllQubitIndicesSorted();
            foreach (int qubitIndex in allIndices)
            {
                if ((qubitIndex < 0) || (qubitIndex >= this.QuBitsCount))
                {
                    message = "Add Gate: Invalid Qubit index.";
                    return false;
                }
            }

            if (stageIndex == this.Stages.Count)
            {
                // Create a new stage 
                this.Stages.Add(new QuStage());
            }

            QuStage stage = this.Stages[stageIndex];

            // Remove operators at all qubit indices if any, then add the provided gate
            // Dont care how many removed 
            int min = allIndices.Min();
            int max = allIndices.Max();
            for (int qubitIndex = min; qubitIndex  <= max; qubitIndex ++)
            {
                _ = stage.ClearAtQubit(qubitIndex);
            }

            stage.AddAtQubit(this, qubitsIndices, gate, isDrop);
            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Add Gate: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool RemoveGate(int stageIndex, QubitsIndices qubitsIndices, out string message)
    {
        message = string.Empty;
        // Do NOT Allow : stageIndex == this.Stages.Count
        if ((stageIndex < 0) || (stageIndex >= this.Stages.Count))
        {
            message = "Remove Gate: Invalid Stage index.";
            return false;
        }

        var allIndices = qubitsIndices.AllQubitIndicesSorted();
        foreach (int qubitIndex in allIndices)
        {
            if ((qubitIndex < 0) || (qubitIndex >= this.QuBitsCount))
            {
                message = "Remove Gate: Invalid Qubit index.";
                return false;
            }
        }

        try
        {
            QuStage stage = this.Stages[stageIndex];

            // Remove operator at all qubit Indices if any,
            // but there should just only one for unary gates 
            foreach (int qubitIndex in allIndices)
            {
                int removed = stage.ClearAtQubit(qubitIndex);
                //if (removed != 1)
                //{
                //    message = "Remove Gate: Unexpected count of removals.";
                //    return false;
                //}
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Remove Gate: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool PackStages (out string message)
    {
        try
        {
            // Remove all empty stages 
            var toRemove = new List<QuStage>(this.Stages.Count);
            foreach (var stage in this.Stages)
            {
                if (stage.IsEmpty())
                {
                    toRemove.Add(stage);
                }
            }

            foreach (var stage in toRemove)
            {
                this.Stages.Remove(stage);
            }

            this.IsValid = false;
            return this.Validate(out message);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Update Qubit: Exception thrown: " + ex.Message;
            return false;
        }
    }
}
