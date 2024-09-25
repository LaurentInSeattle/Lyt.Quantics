namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStage
{
    public List<QuStageOperator> Elements { get; set; }

    public bool IsComplete => false; // For Now 
}
