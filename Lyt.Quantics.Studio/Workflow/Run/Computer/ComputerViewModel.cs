namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ViewActivationMessage;
using static MessagingExtensions;
using static ToolbarCommandMessage;

public sealed class ComputerViewModel : Bindable<ComputerView>
{
    private readonly QuanticsStudioModel quanticsStudioModel;
    private readonly IToaster toaster;
    private readonly IDialogService dialogService;

    private bool isLoaded;
    private bool needsToLoadModel;

    public ComputerViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.toaster = App.GetRequiredService<IToaster>();
        this.dialogService = App.GetRequiredService<IDialogService>();

        // Collections of Qubits and stages view models 
        this.Qubits = [];
        this.Stages = [];

        // Subscribtion processed locally 
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommandMessage);
        this.Messenger.Subscribe<QubitChangedMessage>(this.OnQubitChangedMessage);
        this.Messenger.Subscribe<ModelStructureUpdateMessage>(this.OnModelStructureUpdateMessage);
        this.Messenger.Subscribe<ModelResultsUpdateMessage>(this.OnModelResultsUpdateMessage);
        this.Messenger.Subscribe<ModelUpdateErrorMessage>(this.OnModelUpdateErrorMessage);
        this.Messenger.Subscribe<GateEditMessage>(this.OnGateEditMessage);
    }

    private void OnGateEditMessage(GateEditMessage message)
    {
        if (message.GateViewModel is null)
        {
            return;
        }

        // Run modal dialog 
        if (this.dialogService is DialogService modalService)
        {
            GateEditDialogModel gateEditDialogModel = new();
            modalService.RunModal<GateEditDialog, GateViewModel>(
                this.View.ToasterHost,
                gateEditDialogModel,
                this.OnGateEditClose,
                message.GateViewModel);
        }
    }

    private void OnGateEditClose(object sender, bool close)
    {
        if (sender is not GateEditDialogModel gateEditDialogModel)
        {
            return;
        }

        // Grab the data 

        // Do the stuff 
        Debug.WriteLine("OnGateEditClose");
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

        this.isLoaded = true;
        if (this.NeedsToLoadModel)
        {
            this.InitializeModelOnUi();
        }
    }

    public override void Activate(object? parameter)
    {
        if (parameter is not ComputerActivationParameter computerActivationParameter)
        {
            throw new InvalidOperationException("Invalid parameter for ComputerViewModel");
        }

        switch (computerActivationParameter.ActivationKind)
        {
            default:
            case ComputerActivationParameter.Kind.Back:
                Schedule.OnUiThread(
                    100, () =>
                    {
                        // Coming back after save, there should be nothing to do 
                        // TODO: Need to understand why this fixes the issue of deleted display 
                        // Avalonia ? 
                        this.PackStagesOnUi();

                        // Needed: Computer name and description might have been changed 
                        // in the save dialog. 
                        this.RefreshInfoFields();
                    }, DispatcherPriority.ApplicationIdle);
                break;

            case ComputerActivationParameter.Kind.New:
                this.CreateBlank();
                break;

            case ComputerActivationParameter.Kind.Resource:
                this.CreateFromResource(computerActivationParameter.Name);
                break;

            case ComputerActivationParameter.Kind.Document:
                if (computerActivationParameter.QuComputer is not null)
                {
                    this.CreateFromDocument(computerActivationParameter.QuComputer);
                }
                break;
        }
    }

    private void CreateBlank()
    {
        try
        {
            if (this.quanticsStudioModel.CreateBlank(out string message))
            {
                return;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Reset: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void CreateFromResource(string computerName)
    {
        try
        {
            if (this.quanticsStudioModel.CreateFromResource(computerName, out string message))
            {
                return;
            }

            throw new InvalidOperationException(message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Load Resource: Exception thrown: " + ex.Message;
            this.toaster.Show("Load Resource", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void CreateFromDocument(QuComputer quComputer)
    {
        try
        {
            if (this.quanticsStudioModel.CreateFromDocument(quComputer, out string message))
            {
                return;
            }

            throw new InvalidOperationException(message);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Load Document: Exception thrown: " + ex.Message;
            this.toaster.Show("Load Document", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void OnModelUpdateErrorMessage(ModelUpdateErrorMessage message)
        => this.toaster.Show("Error", message.Message, 4_000, InformationLevel.Error);

    private void OnModelResultsUpdateMessage(ModelResultsUpdateMessage message)
    {
        try
        {
            this.quanticsStudioModel.HideMinibarsComputerState = false;
            var computer = this.quanticsStudioModel.QuComputer;
            int qubitCount = computer.QuBitsCount;

            // All Stages need to update the qubits probabilities 
            foreach (var stage in this.Stages)
            {
                stage.UpdateGatesAndMinibars();
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Model Structure Update: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void AddEmptyStageOnUi()
    {
        // Create an empty stage for the very first qubit and each time we populate 
        // the last stage so that we can drop new gates into it 
        // We are always adding stages at the end of the circuit (for now) therefore
        // the new stage index is the current count of stages 
        int currentStageCount = this.Stages.Count;
        this.Stages.Add(new StageViewModel(currentStageCount, this.quanticsStudioModel));
    }

    private void RefreshInfoFields()
    {
        // Refresh Info fields
        var computer = this.quanticsStudioModel.QuComputer;
        this.Name = computer.Name;
        this.Description = computer.Description;
    }

    private void PackStagesOnUi()
    {
        // Remove all last stages that are beyond the current model stage count.
        var computer = this.quanticsStudioModel.QuComputer;
        var toRemove = new List<StageViewModel>(computer.Stages.Count);
        int computerStageCount = computer.Stages.Count;
        for (int i = this.Stages.Count - 1; i >= 0; --i)
        {
            if (i >= computerStageCount)
            {
                toRemove.Add(this.Stages[i]);
            }
        }

        foreach (var stage in toRemove)
        {
            this.Stages.Remove(stage);
        }

        // All stages may need an update 
        foreach (var stage in this.Stages)
        {
            stage.UpdateGatesAndMinibars();
        }

        AddEmptyStageOnUi();
    }

    private void OnModelStructureUpdateMessage(ModelStructureUpdateMessage message)
    {
        try
        {
            if (message.ModelLoaded)
            {
                Dispatch.OnUiThread(() => this.InitializeModelOnUi(), DispatcherPriority.ApplicationIdle);
                return;
            }

            this.quanticsStudioModel.HideMinibarsComputerState = true;
            var computer = this.quanticsStudioModel.QuComputer;
            int qubitCount = computer.QuBitsCount;

            void AddEmptyStageIfNeeded()
            {
                int currentStageCount = this.Stages.Count;
                if (currentStageCount == 0)
                {
                    AddEmptyStageOnUi();
                }
                else if (currentStageCount == computer.Stages.Count)
                {
                    var lastStage = this.Stages[^1];
                    if (!lastStage.IsEmpty)
                    {
                        AddEmptyStageOnUi();
                    }
                }
            }

            if (message.StagePacked)
            {
                this.PackStagesOnUi();
            }
            else if (message.QubitsChanged)
            {
                if (this.Qubits.Count < qubitCount)
                {
                    // Create the very first empty stage when the first qubit is created
                    // The first qubit cannot be deleted so we will do that only once
                    if (this.Qubits.Count == 0)
                    {
                        AddEmptyStageOnUi();
                    }

                    // Create one new Qubit View, stages are unchanged 
                    int addedQubitIndex = qubitCount - 1;
                    this.Qubits.Add(new QubitViewModel(addedQubitIndex));

                }
                else if (this.Qubits.Count > qubitCount)
                {
                    // Remove last Qubit View 
                    int removedQubitIndex = qubitCount;
                    this.Qubits.RemoveAt(removedQubitIndex);
                    this.PackStagesOnUi();
                }
                else
                {
                    // No change ? 
                    this.toaster.Show("Unexpected Error", "Conflicting QuBits count", 4_000, InformationLevel.Error);
                }
            }
            else if (message.StageChanged)
            {
                // Make sure to update only the stages that are still there,
                // that have not been removed in the loop above 
                if (message.IndexStageChanged < this.Stages.Count)
                {
                    var stage = this.Stages[message.IndexStageChanged];
                    stage.UpdateGatesAndMinibars();
                }

                // All qubit states need to clear 
                foreach (var stage in this.Stages)
                {
                    stage.UpdateUiMinibars();
                }

                // Check if we need to create a new UI stage so that we can drop new gates 
                AddEmptyStageIfNeeded();
            }
            else
            {
                string uiMessage = "Model Structure Update: Logic error ";
                this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Model Structure Update: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void InitializeModelOnUi()
    {
        if (!this.isLoaded)
        {
            this.NeedsToLoadModel = true;
            return;
        }

        var computer = this.quanticsStudioModel.QuComputer;
        int qubitCount = computer.QuBitsCount;
        int stageCount = computer.Stages.Count;

        // Info fields
        this.RefreshInfoFields();

        // Create QuBits UI 'swim lanes'
        this.Qubits = [];
        for (int qubitIndex = 0; qubitIndex < qubitCount; qubitIndex++)
        {
            this.Qubits.Add(new QubitViewModel(qubitIndex));
        }

        // Create Stage UI 
        this.Stages = [];
        for (int stageIndex = 0; stageIndex < stageCount; stageIndex++)
        {
            var stageViewModel = new StageViewModel(stageIndex, this.quanticsStudioModel);
            this.Stages.Add(stageViewModel);
        }

        Schedule.OnUiThread(
            100,
            () =>
            {
                for (int stageIndex = 0; stageIndex < stageCount; stageIndex++)
                {
                    var stageViewModel = this.Stages[stageIndex];
                    stageViewModel.UpdateGatesAndMinibars();
                }

                this.AddEmptyStageOnUi();

                this.NeedsToLoadModel = false;
            }, DispatcherPriority.ApplicationIdle);
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
                if (message.CommandParameter is bool hide)
                {
                    this.HideProbabilities(hide);
                }

                break;

            case ToolbarCommand.PackStages: this.OnPackStages(); break;

            case ToolbarCommand.Reset: this.OnReset(); break;

            case ToolbarCommand.Step: this.OnRun(); break;

            case ToolbarCommand.Run: this.OnRun(); break;

            case ToolbarCommand.Save: this.OnSave(); break;

            case ToolbarCommand.Close: this.OnClose(); break;

            case ToolbarCommand.Loop:
                break;
            case ToolbarCommand.Exit:
                break;
            default:
                break;
        }
    }

    private void OnPackStages()
    {
        try
        {
            if (this.quanticsStudioModel.QuComputer.Stages.Count <= 1)
            {
                this.toaster.Show("Already done!", "No stages to pack.", 4_000, InformationLevel.Info);
                return;
            }

            if (!this.quanticsStudioModel.PackStages(out string message))
            {
                this.toaster.Show("Error!", "Failed to pack stages.", 4_000, InformationLevel.Warning);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Reset: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void HideProbabilities(bool hideMinibars)
    {
        // All stages may need an update 
        foreach (var stage in this.Stages)
        {
            this.quanticsStudioModel.HideMinibarsUserOption = hideMinibars;
            stage.UpdateUiMinibars();
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

    private void OnSave()
    {
        try
        {
            ActivateView(ActivatedView.Save);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Save: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void OnClose()
    {
        try
        {
            if (this.quanticsStudioModel.IsDirty)
            {
                var confirmActionParameters = new ConfirmActionParameters
                {
                    Title = "Unsaved Changes!",
                    Message = "This quantum computer has been modified and your latest changes have not been saved.",
                    ActionVerb = "Discard Changes",
                    OnConfirm = this.OnDiscardChangesConfirmed,
                    InformationLevel = InformationLevel.Warning,
                };

                this.dialogService.Confirm(this.View.ToasterHost, confirmActionParameters);
            }
            else
            {
                // No UI confirmation needed if no changes made
                ActivateView(ActivatedView.Load);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Close: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void OnDiscardChangesConfirmed(bool confirmed)
    {
        if (!confirmed)
        {
            // Move to save view 
            ActivateView(ActivatedView.Save);
            return;
        }

        // changes will be lost
        ActivateView(ActivatedView.Load);
    }

    private void UpdateQubit(int index, QuState newState)
    {
        if ((index <= QuanticsStudioModel.MaxQubits) || (index < 0))
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
        if (count < QuanticsStudioModel.MaxQubits)
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
                string.Format("Add Qubit: Max {0}!", QuanticsStudioModel.MaxQubits),
                string.Format("This Quantum Computer implementation is limited to {0} Qubits...", QuanticsStudioModel.MaxQubits),
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

#pragma warning disable CA1822 // Mark members as static
    public bool CanDrop(Point _, GateViewModel gateViewModel)
#pragma warning restore CA1822 
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

#pragma warning disable CA1822 // Mark members as static
    public void OnDrop(Point _, GateViewModel gateViewModel)
#pragma warning restore CA1822 
    {
        if (gateViewModel.IsToolbox)
        {
            return;
        }

        Debug.WriteLine("ComputerViewModel: OnDrop");
        gateViewModel.Remove();
    }

    public bool NeedsToLoadModel
    {
        get => this.needsToLoadModel;
        set => this.needsToLoadModel = value;
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

    /// <summary> The name of the computer currently edited.</summary>
    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    /// <summary> The description of the computer currently edited.</summary>
    public string Description { get => this.Get<string>()!; set => this.Set(value); }
}
