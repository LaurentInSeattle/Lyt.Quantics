namespace Lyt.Quantics.Studio.Model;

public sealed partial class QuanticsStudioModel : ModelBase
{
    [JsonIgnore]
    public QuComputer QuComputer { get; private set; }

    public bool AddQubit(int count, out string message) 
        => this.QuComputer.AddQubit(count, out message);    

    public bool RemoveQubit(int count, out string message)
        => this.QuComputer.RemoveQubit(count, out message);

    public bool UpdateQubit(int index, QuState newState, out string message)
        => this.QuComputer.UpdateQubit(index, newState, out message);

    public bool AddGate(int stageIndex, int qubitIndex, Gate gate, out string message)
        => this.QuComputer.AddGate(stageIndex, qubitIndex, gate, out message);
}
