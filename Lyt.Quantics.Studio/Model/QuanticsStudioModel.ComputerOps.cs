namespace Lyt.Quantics.Studio.Model;

using static ModelStructureUpdateMessage; 

public sealed partial class QuanticsStudioModel : ModelBase
{
    [JsonIgnore]
    public bool HideMinibars { get; set; }

    [JsonIgnore]
    public QuComputer QuComputer { get; private set; }

    public bool CreateBlank(out string message)
    {
        message = string.Empty;
        try
        {
            // Initialize a 'blank' computer , starting with two (empty) qubits 
            this.QuComputer = new("Untitled", "New quantum computer project.");
            bool status = this.QuComputer.AddQubit(0, out message);
            if (status)
            {
                status = this.QuComputer.AddQubit(1, out message);
                if (status)
                {
                    this.Messenger.Publish(MakeModelLoaded());
                    return true;
                } 
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Create New: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool CreateFromResource(string computerName, out string message)
    {
        try
        {
            Debugger.Break();
            message = string.Empty;
            var builtInComputers = QuanticsStudioModel.BuiltInComputers;
            this.QuComputer = builtInComputers[computerName];
            bool status = this.QuComputer.Validate(out message);
            if (status)
            {
                this.Messenger.Publish(MakeModelLoaded());
                return true;
            }

            return false;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            message = "Create New: Exception thrown: " + ex.Message;
            return false;
        }
    }

    public bool AddQubit(int count, out string message)
    {
        bool status = this.QuComputer.AddQubit(count, out message);
        if (status)
        {
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
            this.Messenger.Publish(MakeStageChanged(stageIndex));
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool Reset ()
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
                    this.Messenger.Publish(new ModelResultsUpdateMessage());
                    return true;
                } 
            }
        }

        this.PublishError(message);
        return false;
    }

    public bool Run ()
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
                        status = this.QuComputer.Run(out message);
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
