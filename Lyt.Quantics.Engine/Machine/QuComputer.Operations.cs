namespace Lyt.Quantics.Engine.Machine;

public sealed partial class QuComputer
{
    public bool AddQubit(int count, out string message)
    {
        try
        {
            this.InitialStates.Add(QuState.Zero);
            this.QuBitsCount = this.InitialStates.Count;
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

    public bool RemoveQubit(int count, out string message)
    {
        try
        {
            int qubitIndex = count - 1;
            this.InitialStates.RemoveAt(qubitIndex);
            this.QuBitsCount = this.InitialStates.Count;

            // For all stages : Remove all operators 'touching' this qubit index 
            foreach (var stage in this.Stages)
            {
                stage.ClearAtQubit(qubitIndex);
            }

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

    public bool AddGate(int stageIndex, int qubitIndex, Gate gate, out string message)
    {
        message = string.Empty;
        // Allow : stageIndex == this.Stages.Count
        if ((stageIndex < 0) || (stageIndex > this.Stages.Count))
        {
            message = "Add Gate: Invalid Stage index.";
            return false;
        }

        if ((qubitIndex < 0) || (qubitIndex >= this.QuBitsCount))
        {
            message = "Add Gate: Invalid Qubit index.";
            return false;
        }

        try
        {
            if (stageIndex == this.Stages.Count)
            {
                // Create a new stage 
                this.Stages.Add(new QuStage());
            }

            QuStage stage = this.Stages[stageIndex];

            // Remove operator at qubitIndex if any, then add the provided gate
            // Dont care how many removed 
            _ = stage.ClearAtQubit(qubitIndex);
            stage.AddAtQubit(this, qubitIndex, gate);

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Add Gate: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool RemoveGate(int stageIndex, int qubitIndex, Gate gate, out string message)
    {
        message = string.Empty;

        // Do NOT Allow : stageIndex == this.Stages.Count
        if ((stageIndex < 0) || (stageIndex >= this.Stages.Count))
        {
            message = "Remove Gate: Invalid Stage index.";
            return false;
        }

        if ((qubitIndex < 0) || (qubitIndex >= this.QuBitsCount))
        {
            message = "Remove Gate: Invalid Qubit index.";
            return false;
        }

        try
        {
            QuStage stage = this.Stages[stageIndex];

            // Remove operator at qubitIndex if any, but there should just only one
            int removed = stage.ClearAtQubit(qubitIndex);
            if (removed != 1)
            {
                message = "Remove Gate: Unexpected count of removals.";
                return false;
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
}
