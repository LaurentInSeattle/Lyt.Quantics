﻿namespace Lyt.Quantics.Studio.Workflow.Save;

using Tmds.DBus.Protocol;
using static MessagingExtensions;
using static ToolbarCommandMessage;
using static ViewActivationMessage;

public sealed class DoSaveViewModel : Bindable<DoSaveView>
{
    private readonly QuanticsStudioModel quanticsStudioModel;

    public DoSaveViewModel()
    {
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommandMessage);
    }

    private void OnToolbarCommandMessage(ToolbarCommandMessage message)
    {
        if (message.Command == ToolbarCommand.SaveToFile)
        {
            this.TrySave();
        }
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        var computer = this.quanticsStudioModel.QuComputer;
        this.Name = computer.Name;
        this.Description = computer.Description;
    }

    private bool Validate(out string message)
        => this.quanticsStudioModel.ValidateComputerMetadata(
            this.Name, this.Description, out message);

    private void TrySave()
    {
        if (!this.Validate(out string message))
        {
            this.ValidationMessage = message;
            return;
        }

        // Save to model 
        this.ValidationMessage = string.Empty;
        string newName = this.Name.Trim();
        string description = this.Description.Trim();
        if (this.quanticsStudioModel.SaveComputerMetadata(newName, description, out message))
        {
            // Save to file 
            string? pathName = this.quanticsStudioModel.SaveComputerToFile(out message);
            if (string.IsNullOrWhiteSpace(pathName))
            {
                // Fail: 
                this.ValidationMessage = message;
                return;
            }

            // Success: Toast Happy 
            var toaster = App.GetRequiredService<IToaster>();
            toaster.Show("Saved", "Saved as " +pathName , 4_000, InformationLevel.Success);

            // Go back to run the model 
            ActivateView(
                ActivatedView.Run,
                new ComputerActivationParameter(ComputerActivationParameter.Kind.Back));
        }
        else
        {
            // Fail: 
            this.ValidationMessage = message;
            return;
        }
    }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Description { get => this.Get<string>()!; set => this.Set(value); }

    public string ValidationMessage { get => this.Get<string>()!; set => this.Set(value); }
}
