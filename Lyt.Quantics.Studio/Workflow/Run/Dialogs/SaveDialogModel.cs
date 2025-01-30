namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class SaveDialogModel : DialogBindable<SaveDialog, object>
{
    private readonly QsModel quanticsStudioModel;

    public SaveDialogModel()
    {
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
        this.Title = "Save your Project";
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        // Force property changed 
        this.SaveButtonIsEnabled = true;
        this.SaveButtonIsEnabled = false;

        var computer = this.quanticsStudioModel.QuComputer;
        this.Name = computer.Name;
        this.Description = computer.Description;
        this.ValidationMessage = string.Empty;
        this.Validate(withOverwrite: true, out string _);
    }

#pragma warning disable IDE0051 // Remove unused private members

    private void OnOverwrite(object? _)
    {
        if (this.TrySave(withOverwrite: true))
        {
            this.TrySaveAndClose();
        }
    }

    private void OnSave(object? _)
    {
        if (this.TrySave(withOverwrite: false))
        {
            this.TrySaveAndClose();
        }
    }

    private void OnCancel(object? _) => this.Cancel();

#pragma warning restore IDE0051 // Remove unused private members

    private bool TrySave(bool withOverwrite)
    {
        if (!this.Validate(withOverwrite, out string _))
        {
            return false;
        }

        // Save to model 
        this.ValidationMessage = string.Empty;
        string newName = this.Name.Trim();
        string description = this.Description.Trim();
        if (this.quanticsStudioModel.SaveComputerMetadata(
            newName, description, withOverwrite, out string message))
        {
            // Save to file 
            string? pathName = this.quanticsStudioModel.SaveComputerToFile(withOverwrite, out message);
            if (string.IsNullOrWhiteSpace(pathName))
            {
                // Fail: 
                this.ValidationMessage = message;
                return false;
            }

            // Success: Toast Happy 
            var toaster = App.GetRequiredService<IToaster>();
            toaster.Show("Saved", "Saved as: " + pathName, 4_000, InformationLevel.Success);

            // Go back 
            return true;
        }
        else
        {
            // Fail: 
            this.ValidationMessage = message;
            return false;
        }
    }

    private bool Validate(bool withOverwrite, out string message)
    {
        bool validated =
            this.quanticsStudioModel.ValidateComputerMetadata(
            this.Name, this.Description, withOverwrite, out message);
        this.SaveButtonIsEnabled = validated;
        this.ValidationMessage = validated ? string.Empty : message;
        return validated;
    }

    #region Bound  Properties 

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Description { get => this.Get<string>()!; set => this.Set(value); }

    public string ValidationMessage { get => this.Get<string>()!; set => this.Set(value); }

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public ICommand OverwriteCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CancelCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public bool SaveButtonIsEnabled
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.View.SaveOverwriteButton.IsDisabled = !value;
            this.View.SaveAsButton.IsDisabled = !value;
        }
    }

    #endregion Bound Properties 
}
