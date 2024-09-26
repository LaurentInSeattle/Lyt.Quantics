namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStageOperator
{
    public List<int> QuBitIndices { get; set; } = [];

    public string GateKey { get; set; } = "I";

    public bool Validate(out string message)
    {
        message = string.Empty;

        if ( !GateFactory.AvailableProducts.ContainsKey(this.GateKey) )
        {
            message = "Stage operator is using an unknown key: " + this.GateKey;
            return false;
        }

        return true;
    }
}
