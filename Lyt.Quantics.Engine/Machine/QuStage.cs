namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStage
{
    public QuStage() { /* Required for deserialization */ }

    public List<QuStageOperator> Operators { get; set; } = [];

    public bool IsComplete => false; // For Now 
}
