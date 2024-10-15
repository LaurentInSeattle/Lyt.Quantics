namespace Lyt.Quantics.Studio.Model;

public sealed partial class QuanticsStudioModel : ModelBase
{
    [JsonIgnore]
    public QuComputer QuComputer { get; private set; }

    public bool AddQubit(int count, out string message)
    {
        bool status = this.QuComputer.AddQubit(count, out message);
        if (status)
        {
            this.Messenger.Publish(new ModelStructureUpdateMessage(qubitsChanged: true));
        }

        return status;
    } 

    public bool RemoveQubit(int count, out string message)
    {
        bool status = this.QuComputer.RemoveQubit(count, out message);
        if (status)
        {
            this.Messenger.Publish(new ModelStructureUpdateMessage(qubitsChanged: true));
        }

        return status;
    }    

    public bool UpdateQubit(int index, QuState newState, out string message)
    {
        bool status = this.QuComputer.UpdateQubit(index, newState, out message);
        if (status)
        {
            this.Messenger.Publish(new ModelResultsUpdateMessage());
        }

        return status;
    }    

    public bool AddGate(int stageIndex, int qubitIndex, Gate gate, out string message)
    {
        bool status = this.QuComputer.AddGate(stageIndex, qubitIndex, gate, out message);
        if (status)
        {
            this.Messenger.Publish(
                new ModelStructureUpdateMessage(
                    qubitsChanged: false, stageChanged:true, stageIndex));
        }

        return status;
    }

    public bool RemoveGate(int stageIndex, int qubitIndex, Gate gate, out string message)
    {
        bool status = this.QuComputer.RemoveGate(stageIndex, qubitIndex, gate, out message);
        if (status)
        {
            this.Messenger.Publish(
                new ModelStructureUpdateMessage(
                    qubitsChanged: false, stageChanged: true, stageIndex));
        }

        return status;
    }


}
