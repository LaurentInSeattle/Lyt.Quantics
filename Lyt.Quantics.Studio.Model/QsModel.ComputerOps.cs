namespace Lyt.Quantics.Studio.Model;

using static ModelStructureUpdateMessage;

public sealed partial class QsModel : ModelBase
{
    [JsonIgnore]
    public bool HideMinibarsComputerState { get; set; }

    [JsonIgnore]
    public bool HideMinibarsUserOption { get; set; }

    [JsonIgnore]
    public QuComputer QuComputer { get; private set; }

    [JsonIgnore]
    public List<bool> QuBitMeasureStates { get; private set; } = [];

    public bool ShouldMeasureAllQubits
    {
        get
        {
            int total = this.QuBitMeasureStates.Count;
            if ( total != this.QuComputer.QuBitsCount)
            {
                return false;
            }

            int countMeasured = 
                (from state in this.QuBitMeasureStates where state select state).Count();
            return countMeasured == total;
        }
    }

    public bool ShouldMeasureNoQubits
    {
        get
        {
            int total = this.QuBitMeasureStates.Count;
            if (total != this.QuComputer.QuBitsCount)
            {
                return false;
            }

            int countMeasured =
                (from state in this.QuBitMeasureStates where state select state).Count();
            return countMeasured == 0;
        }
    }

    public bool AddQubitAtEnd(int count, out string message)
    {
        bool status = this.QuComputer.AddQubit(count, out message);
        if (status)
        {
            this.QuBitMeasureStates.Add(true);
            this.IsDirty = true;
            this.Messenger.Publish(MakeQubitsChanged());
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool RemoveLastQubit(int count, out string message)
    {
        bool status = this.QuComputer.RemoveQubit(count, out message);
        if (status)
        {
            this.QuBitMeasureStates.RemoveAt(this.QuBitMeasureStates.Count - 1);
            this.IsDirty = true;
            this.Messenger.Publish(MakeQubitsChanged());
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool UpdateQubitMeasureState(int index, bool value , out string message)
    {
        message = string.Empty;
        if ((index < 0) || (index >= this.QuBitMeasureStates.Count))
        {
            message = "Invalid qubit index";
            return false;
        } 

        this.QuBitMeasureStates[index] = value;
        this.Messenger.Publish(new ModelMeasureStatesUpdateMessage());
        return true;
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

    public bool AddGate(int stageIndex, QubitsIndices qubitsIndices, Gate gate, bool isDrop, out string message)
    {
        bool status = this.QuComputer.AddGate(stageIndex, qubitsIndices, gate, isDrop, out message);
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

    public bool RemoveGate(int stageIndex, QubitsIndices qubitsIndices, Gate gate, out string message)
    {
        bool status = this.QuComputer.RemoveGate(stageIndex, qubitsIndices, gate, out message);
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

    public bool Run(bool runSingleStage)
    {
        this.QuComputer.RunSingleStage = runSingleStage;
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

    private void InitializeMeasureStates()
    {
        this.QuBitMeasureStates.Clear();
        for (int i = 0; i < this.QuComputer.QuBitsCount; i++)
        {
            this.QuBitMeasureStates.Add(true);
        }
    }
}
