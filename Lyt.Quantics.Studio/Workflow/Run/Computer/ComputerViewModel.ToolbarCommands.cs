namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static MessagingExtensions;
using static ToolbarCommandMessage;

public sealed partial class ComputerViewModel : ViewModel<ComputerView>
{
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

    private void AddQubit(int count)
    {
        if (count < QuRegister.MaxQubits)
        {
            if (!this.quanticsStudioModel.AddQubitAtEnd(out string message))
            {
                this.toaster.Show("Failed to Add Qubit!", message, 4_000, InformationLevel.Error);
            }
        }
        else
        {
            // message 
            this.toaster.Show(
                string.Format("Add Qubit: Max {0}!", QuRegister.MaxQubits),
                string.Format("This Quantum Computer implementation is limited to {0} Qubits...", QuRegister.MaxQubits),
                4_000, InformationLevel.Error);
        }
    }

    private void RemoveQubit(int count)
    {
        if (this.quanticsStudioModel.QuComputer.Stages.Count == 0)
        {
            this.toaster.Show(
                "Empty Computer", "No stages, Qubit cannot be removed.",
                4_000, InformationLevel.Warning);
            return;
        }

        if (count == 1)
        {
            this.toaster.Show(
                "Last Qubit!", "The last Qubit cannot be removed.",
                4_000, InformationLevel.Warning);
            return;
        }

        if (this.quanticsStudioModel.RemoveLastQubit(out string message))
        {
        }
        else
        {
            this.toaster.Show("Failed to Remove last Qubit!", message, 4_000, InformationLevel.Error);
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

    private void OnReset()
    {
        try
        {
            if (this.quanticsStudioModel.Reset())
            {
                this.toaster.Show("Ready!", "Ready to Run!", 4_000, InformationLevel.Success);
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
            if (this.quanticsStudioModel.QuComputer.QuBitsCount <= QuRegister.UiThreadedRunAtQubits)
            {
                if (this.quanticsStudioModel.Run(runUsingKroneckerProduct: false))
                {
                    this.toaster.Show("Complete!", "Successful single Run! ", 4_000, InformationLevel.Success);
                }
            }
            else
            {
                // Run modal dialog to save computer model - no parameters, no closing action  
                if (this.dialogService is DialogService modalService)
                {
                    modalService.RunViewModelModal(this.View.ToasterHost, new RunDialogModel());
                }
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
            // Run modal dialog to save computer model - no parameters, no closing action  
            if (this.dialogService is DialogService modalService)
            {
                modalService.RunViewModelModal(
                    this.View.ToasterHost, new SaveDialogModel(), this.OnSaveClosing);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            string uiMessage = "Save: Exception thrown: " + ex.Message;
            this.toaster.Show("Unexpected Error", uiMessage, 4_000, InformationLevel.Error);
        }
    }

    private void OnSaveClosing(object _, bool handled )
    {
        if (handled)
        {
            this.RefreshInfoFields();
        } 
    }

    private void OnClose()
    {
        try
        {
            var keyboard = App.GetRequiredService<Keyboard>();
            bool isShifted = keyboard.Modifiers.HasFlag(KeyModifiers.Shift);

            // No confirmation if we "shift close" 
            if ((!isShifted) && this.quanticsStudioModel.IsDirty)
            {
                // ( NOT shifted ) AND ( dirty ) 
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
                // No UI confirmation needed if shifted - OR - if no changes made
                Select(ActivatedView.Load);
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
            // Run modal dialog to save computer model - no parameters, no closing action  
            if (this.dialogService is DialogService modalService)
            {
                // Need to dismiss in some cases
                modalService.Dismiss();
                modalService.RunViewModelModal(this.View.ToasterHost, new SaveDialogModel());
            }

            return;
        }

        // Clear dirty flag, changes will be lost on exit 
        this.quanticsStudioModel.Clean();
        Select(ActivatedView.Load);
    }

    public bool OnClicked(bool isLeft)
    {
        var stage = this.Stages[isLeft ? 0 : this.Stages.Count - 1];
        var stageView = stage.View;
        stageView.BringIntoView();
        return true; 
    }
}
