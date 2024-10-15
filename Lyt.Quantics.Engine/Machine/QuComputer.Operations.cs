namespace Lyt.Quantics.Engine.Machine;

public sealed partial class QuComputer
{
    public bool AddQubit(int count, out string message)
    {
        this.InitialStates.Add(QuState.Zero);
        this.QuBitsCount = this.InitialStates.Count;
        // TODO: Stages  
        if (this.Stages.Count == 0)
        {
            QuStage quStage = new();
            this.Stages.Add(quStage);
        }

        this.IsValid = false;
        return this.Validate(out message);
    }

    public bool RemoveQubit(int count, out string message)
    {
        this.InitialStates.RemoveAt(count - 1);
        this.QuBitsCount = this.InitialStates.Count;

        // TODO: Stages  
        this.IsValid = false;
        return this.Validate(out message);
    }

    public bool UpdateQubit(int index, QuState newState, out string message)
    {
        if ((index < 0) || (index >= this.QuBitsCount))
        {
            message = "Invalid Qubit index.";
            return false;
        }

        this.InitialStates[index] = newState;
        this.IsValid = false;
        return this.Validate(out message);
    }

    public bool AddGate(int stageIndex, int qubitIndex, Gate gate, out string message)
    {
        // TODO ! 
        message = string.Empty;
        return true;
    }

    public bool RemoveGate(int stageIndex, int qubitIndex, Gate gate, out string message)
    {
        // TODO ! 
        message = string.Empty;
        return true;
    }
}
