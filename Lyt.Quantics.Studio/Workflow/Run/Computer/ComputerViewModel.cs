namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ToolbarCommandMessage;

public sealed class ComputerViewModel : Bindable<ComputerView>
{
    public const int MaxQubits = 10; // For now ~ 10 could be doable ? 

    private readonly QuanticsStudioModel quanticsStudioModel;
    private readonly IToaster toaster;

    public ComputerViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.toaster = App.GetRequiredService<IToaster>();
        this.Qubits = [];
        this.Stages = [];
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommandMessage);
        this.Messenger.Subscribe<QubitChangedMessage>(this.OnQubitChangedMessage);
        this.Messenger.Subscribe<ModelStructureUpdateMessage>(this.OnModelStructureUpdateMessage);
        this.Messenger.Subscribe<ModelResultsUpdateMessage>(this.OnModelResultsUpdateMessage);
        this.Messenger.Subscribe<ModelUpdateErrorMessage>(this.OnModelUpdateErrorMessage);
    }

    protected override void OnViewLoaded()
    {
        Schedule.OnUiThread(
            5_000, () =>
            {

                this.toaster.Dismiss();
                this.toaster.Host = this.View.ToasterHost;

                // Relocate the location of the toasts for better readability. 
                if (this.toaster.View is Control control)
                {
                    control.HorizontalAlignment = HorizontalAlignment.Right;
                }
            }, DispatcherPriority.ApplicationIdle);

        // FOW NOW: This is only valid for starting from scratch,
        // and will be different if starting from file
        // Initialize, starting with two (empty) qubits 
        _ = this.quanticsStudioModel.AddQubit(0, out string _);
        _ = this.quanticsStudioModel.AddQubit(1, out string _);

    }

    private void OnModelUpdateErrorMessage(ModelUpdateErrorMessage message)
        => this.toaster.Show("Error", message.Message, 4_000, InformationLevel.Error);

    private void OnModelResultsUpdateMessage(ModelResultsUpdateMessage message)
    {
        try
        {
            var computer = this.quanticsStudioModel.QuComputer;
            int qubitCount = computer.QuBitsCount;

            // All Stages need to update the qubits probabilities 
            foreach (var stage in this.Stages)
            {
                stage.Update();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Model Structure Update: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void OnModelStructureUpdateMessage(ModelStructureUpdateMessage message)
    {
        try
        {
            var computer = this.quanticsStudioModel.QuComputer;
            int qubitCount = computer.QuBitsCount;

            void AddEmptyStage()
            {
                // Create an empty stage for the very first qubit and each time we populate 
                // the last stage so that we can drop new gates into it 
                // We are always adding stages at the end of the circuit (for now) therefore
                // the new stage index is the current count of stages 
                int currentStageCount = this.Stages.Count;
                this.Stages.Add(new StageViewModel(currentStageCount, this.quanticsStudioModel));
            }

            void AddEmptyStageIfNeeded()
            {
                int currentStageCount = this.Stages.Count;
                if (currentStageCount == 0)
                {
                    AddEmptyStage();
                }
                else if (currentStageCount == computer.Stages.Count)
                {
                    var lastStage = this.Stages[^1];
                    if (!lastStage.IsEmpty)
                    {
                        AddEmptyStage();
                    }
                }
            }

            bool RemoveLastEmptyStageIfNeeded()
            {
                int stageCount = this.Stages.Count;
                if (stageCount <= 1)
                {
                    return false;
                }

                int last = stageCount - 1;
                if (this.Stages[last].IsEmpty && this.Stages[last - 1].IsEmpty)
                {
                    var stageToRemove = this.Stages[last];
                    this.Stages.Remove(stageToRemove);
                    return true;
                }

                return false;
            }

            if (message.QubitsChanged)
            {
                if (this.Qubits.Count < qubitCount)
                {
                    // Create one new Qubit View, stages are unchanged 
                    int addedQubitIndex = qubitCount - 1;
                    this.Qubits.Add(new QubitViewModel(addedQubitIndex));
                    if (addedQubitIndex == 0)
                    {
                        AddEmptyStageIfNeeded();
                    }
                }
                else if (this.Qubits.Count > qubitCount)
                {
                    // Remove last Qubit View 
                    int removedQubitIndex = qubitCount;
                    this.Qubits.RemoveAt(removedQubitIndex);

                    // Update stages later: We can add / remove some 
                }
                else
                {
                    // No change ? 
                    this.toaster.Show("Unexpected Error", "Conflicting QuBits count", 4_000, InformationLevel.Error);
                }
            }

            // Check if we need to remove the last UI stage if gates have been removed
            // We may need to remove more than one, typically when deleting qubits 
            while (RemoveLastEmptyStageIfNeeded())
            {
                if (this.Stages.Count == 1)
                {
                    break;
                }
            }

            // Make sure to update only the stages that are still there,
            // that have not been removed in the loop above 
            if (message.StageChanged)
            {
                if (message.IndexStageChanged < this.Stages.Count)
                {
                    var stage = this.Stages[message.IndexStageChanged];
                    stage.Update();
                }
            }
            else if (!message.QubitsChanged)
            {
                // All stages may need an update 
                foreach (var stage in this.Stages)
                {
                    stage.Update();
                }
            }

            // Check if we need to create a new UI stage so that we can drop new gates 
            AddEmptyStageIfNeeded();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Model Structure Update: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void OnQubitChangedMessage(QubitChangedMessage message)
        => this.UpdateQubit(message.Index, message.InitialState);

    private void OnToolbarCommandMessage(ToolbarCommandMessage message)
    {
        int count = this.Qubits.Count;
        switch (message.Command)
        {
            case ToolbarCommand.AddQubit: this.AddQubit(count); break;

            case ToolbarCommand.RemoveQubit: this.RemoveQubit(count); break;

            case ToolbarCommand.HideProbabilities:
                break;
            case ToolbarCommand.Reset: this.OnReset(); break;

            case ToolbarCommand.Step: this.OnRun(); break;

            case ToolbarCommand.Run: this.OnRun(); break;

            case ToolbarCommand.Loop:
                break;
            case ToolbarCommand.Exit:
                break;
            default:
                break;
        }
    }

    private void OnReset()
    {
        try
        {
            if (this.quanticsStudioModel.Reset())
            {
                this.toaster.Show("Ready!", "Ready to Run! (or Step...)", 4_000, InformationLevel.Success);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Reset: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void OnRun()
    {
        try
        {
            if (this.quanticsStudioModel.Run())
            {
                this.toaster.Show("Complete!", "Successful single Run! ", 4_000, InformationLevel.Success);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Run: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void UpdateQubit(int index, QuState newState)
    {
        if ((index <= ComputerViewModel.MaxQubits) || (index < 0))
        {
            if (!this.quanticsStudioModel.UpdateQubit(index, newState, out string message))
            {
                this.toaster.Show("Failed to update Qubit!", message, 4_000, InformationLevel.Error);
            }
        }
        else
        {
            // message 
            this.toaster.Show(
                string.Format("No such Qubit at index: {0}!", index),
                string.Format("Failed to correctly index Qubit at: {0}.", index),
                4_000, InformationLevel.Error);
        }
    }

    private void AddQubit(int count)
    {
        if (count < ComputerViewModel.MaxQubits)
        {
            if (!this.quanticsStudioModel.AddQubit(count, out string message))
            {
                this.toaster.Show("Failed to Add Qubit!", message, 4_000, InformationLevel.Error);
            }
        }
        else
        {
            // message 
            this.toaster.Show(
                string.Format("Add Qubit: Max {0}!", MaxQubits),
                string.Format("This Quantum Computer implementation is limited to {0} Qubits...", MaxQubits),
                4_000, InformationLevel.Error);
        }
    }

    private void RemoveQubit(int count)
    {
        if (count == 1)
        {
            this.toaster.Show(
                "Last Qubit!", "The last Qubit cannot be removed.",
                4_000, InformationLevel.Warning);
        }
        else if (count > 1)
        {
            if (this.quanticsStudioModel.RemoveQubit(count, out string message))
            {
            }
            else
            {
                this.toaster.Show("Failed to Remove last Qubit!", message, 4_000, InformationLevel.Error);
            }
        }
    }

    public bool CanDrop(Point point, GateViewModel gateViewModel)
    {
        if (gateViewModel.IsToolbox)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void OnDrop(Point point, GateViewModel gateViewModel)
    {
        if (gateViewModel.IsToolbox)
        {
            return;
        }

        Debug.WriteLine("ComputerViewModel: OnDrop");
        gateViewModel.Remove();
    }

    public ObservableCollection<QubitViewModel> Qubits
    {
        get => this.Get<ObservableCollection<QubitViewModel>>()!;
        set => this.Set(value);
    }

    public ObservableCollection<StageViewModel> Stages
    {
        get => this.Get<ObservableCollection<StageViewModel>>()!;
        set => this.Set(value);
    }
}
