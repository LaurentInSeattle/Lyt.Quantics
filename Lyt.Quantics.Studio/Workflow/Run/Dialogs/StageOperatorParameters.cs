namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class StageOperatorParameters
{
    public StageOperatorParameters() { }

    /// <summary> Convenience ctor for mutating unary gates. </summary>
    public StageOperatorParameters(int controlIndex, int targetIndex)
    {
        this.ControlQuBitIndices.Add(controlIndex);
        this.TargetQuBitIndices.Add(targetIndex);
    }

    /// <summary> Control Qubit Indices for controlled gates </summary>
    public List<int> ControlQuBitIndices { get; set; } = [];

    /// <summary> Target Qubit Indices for controlled gates and non controlled ones</summary>
    public List<int> TargetQuBitIndices { get; set; } = [];

    public void Clear ()
    {
        this.ControlQuBitIndices.Clear();
        this.TargetQuBitIndices.Clear();
    }
}
