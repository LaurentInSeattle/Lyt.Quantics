namespace Lyt.Quantics.Engine.Machine;

public sealed class QubitsIndices
{
    public QubitsIndices() { }

    public QubitsIndices(QuStageOperator stageOperator) 
    {
        this.ControlQuBitIndices = stageOperator.ControlQuBitIndices;
        this.TargetQuBitIndices = stageOperator.TargetQuBitIndices;
    }

    /// <summary> Convenience ctor for unary gates. </summary>
    public QubitsIndices(int targetIndex) 
        => this.TargetQuBitIndices.Add(targetIndex);

    /// <summary> Convenience ctor for mutating unary gates. </summary>
    public QubitsIndices(int controlIndex, int targetIndex)
    {
        this.ControlQuBitIndices.Add(controlIndex);
        this.TargetQuBitIndices.Add(targetIndex);
    }

    /// <summary> Control Qubit Indices for controlled gates </summary>
    public List<int> ControlQuBitIndices { get; set; } = [];

    /// <summary> Target Qubit Indices for controlled gates and non controlled ones</summary>
    public List<int> TargetQuBitIndices { get; set; } = [];

    public List<int> AllQubitIndicesSorted()
    {
        var allIndices = new List<int>();
        allIndices.AddRange(this.ControlQuBitIndices);
        allIndices.AddRange(this.TargetQuBitIndices);
        return [.. (from index in allIndices orderby index ascending select index)];
    } 

    public void Clear ()
    {
        this.ControlQuBitIndices.Clear();
        this.TargetQuBitIndices.Clear();
    }
}
