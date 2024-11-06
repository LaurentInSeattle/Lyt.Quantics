namespace Lyt.Quantics.Studio.Model;

using static ModelStructureUpdateMessage;
using static FileManagerModel;

public sealed partial class QsModel : ModelBase
{
    [JsonIgnore]
    public bool HideMinibarsComputerState { get; set; }

    [JsonIgnore]
    public bool HideMinibarsUserOption { get; set; }

    [JsonIgnore]
    public QuComputer QuComputer { get; private set; }

    public bool AddQubit(int count, out string message)
    {
        bool status = this.QuComputer.AddQubit(count, out message);
        if (status)
        {
            this.IsDirty = true;
            this.Messenger.Publish(MakeQubitsChanged());
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool RemoveQubit(int count, out string message)
    {
        bool status = this.QuComputer.RemoveQubit(count, out message);
        if (status)
        {
            this.IsDirty = true;
            this.Messenger.Publish(MakeQubitsChanged());
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool PackStages(out string message)
    {
        bool status = this.QuComputer.PackStages(out message);
        if (status)
        {
            this.IsDirty = true;
            this.Messenger.Publish(MakeStagePacked());
        }
        else
        {
            this.PublishError(message);
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
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool AddGate(int stageIndex, int qubitIndex, Gate gate, out string message)
    {
        bool status = this.QuComputer.AddGate(stageIndex, qubitIndex, gate, out message);
        if (status)
        {
            this.IsDirty = true;
            this.Messenger.Publish(MakeStageChanged(stageIndex));
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool RemoveGate(int stageIndex, int qubitIndex, Gate gate, out string message)
    {
        bool status = this.QuComputer.RemoveGate(stageIndex, qubitIndex, gate, out message);
        if (status)
        {
            this.IsDirty = true;
            this.Messenger.Publish(MakeStageChanged(stageIndex));
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool Reset()
    {
        bool status = this.QuComputer.Reset(out string message);
        if (status)
        {
            this.QuComputer.Validate(out message);
            if (status)
            {
                status = this.QuComputer.Build(out message);
                if (status)
                {
                    status = this.QuComputer.Prepare(out message);
                    if (status)
                    {
                        this.Messenger.Publish(new ModelResetMessage());
                        this.Messenger.Publish(new ModelResultsUpdateMessage());
                        return true;
                    }
                }
            }
        }

        this.PublishError(message);
        return false;
    }

    public bool Run()
    {
        bool status = this.QuComputer.Validate(out string message);
        if (status)
        {
            status = this.QuComputer.Build(out message);
            if (status)
            {
                status = this.QuComputer.Prepare(out message);
                if (status)
                {
                    status = this.QuComputer.Run(checkExpected: false, out message);
                    if (status)
                    {
                        this.Messenger.Publish(new ModelResultsUpdateMessage());
                        return true;
                    }
                }
            }
        }

        this.PublishError(message);
        return false;
    }

    private void PublishError(string message)
        => this.Messenger.Publish(new ModelUpdateErrorMessage(message));

}
