﻿namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ViewActivationMessage;
using static MessagingExtensions;
using static ToolbarCommandMessage;

public sealed partial class ComputerViewModel : Bindable<ComputerView>
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

    private void AddQubit(int count)
    {
        if (count < QuRegister.MaxQubits)
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
                string.Format("Add Qubit: Max {0}!", QuRegister.MaxQubits),
                string.Format("This Quantum Computer implementation is limited to {0} Qubits...", QuRegister.MaxQubits),
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
            if (this.quanticsStudioModel.Run(runSingleStage: false))
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

        // Clear dirty flag, changes will be lost on exit 
        this.quanticsStudioModel.Clean();
        ActivateView(ActivatedView.Load);
    }
}