namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ViewActivationMessage;
using static MessagingExtensions;
using static ToolbarCommandMessage;

public sealed partial class ComputerViewModel : Bindable<ComputerView>
{
    private readonly QsModel quanticsStudioModel;
    private readonly IToaster toaster;
    private bool isLoaded;
    private bool needsToLoadModel;

    public ComputerViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
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

    private void UpdateQubit(int index, QuState newState)
    {
        if ((index <= QuRegister.MaxQubits) || (index < 0))
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

#pragma warning disable CA1822 // Mark members as static
    public bool CanDrop(Point _, IGateInfoProvider gateInfoProvider)
        => !gateInfoProvider.IsToolbox;
#pragma warning restore CA1822 

    public void OnDrop(Point _, IGateInfoProvider gateInfoProvider)
    {
        if (gateInfoProvider.IsToolbox)
        {
            return;
        }

        this.Remove(gateInfoProvider);
    }

    private void Remove(IGateInfoProvider gateInfoProvider)
    {
        // Debug.WriteLine("Removing gate: " + gateInfoProvider.Gate.CaptionKey);
        if (!this.quanticsStudioModel.RemoveGate(
            gateInfoProvider.StageIndex, gateInfoProvider.QubitsIndices, gateInfoProvider.Gate, out string message))
        {
            this.toaster.Show(
                "Failed to Remove gate: " + gateInfoProvider.Gate.CaptionKey, message,
                4_000, InformationLevel.Error);
        }
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
