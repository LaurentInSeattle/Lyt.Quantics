namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStageOperator
{
    public List<int> Inputs { get; set; } = [];

    public List<int> Outputs { get; set; } = [];

    public string GateKey { get; set; } = "I"; 
}
