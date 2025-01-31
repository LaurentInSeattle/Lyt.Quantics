namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

using static SharedValidators;

public sealed class SaveDialogModel : DialogBindable<SaveDialog, object>
{
    private readonly QsModel quanticsStudioModel;

    private readonly FormValidator<FileInformation> fileValidator;

    public SaveDialogModel()
    {
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
        this.Title = "Save your Project";
        this.fileValidator =
            new FormValidator<FileInformation>(
                new(FormValidPropertyName: "FormIsValid",
                    FocusFieldName: "NameTextBox",
                    FieldValidators: [NameValidator, DescriptionValidator]));
        this.FileInformation = new();
        this.CanEnter = false; 
    }

    public FileInformation FileInformation { get; private set; }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.ClearForm();
    }

    private void ClearForm()
    {
        var computer = this.quanticsStudioModel.QuComputer;
        this.Name = computer.Name;
        this.Description = computer.Description;
        this.fileValidator.Validate(this);
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
        // Extra validation when about to save 
        if (!this.Validate(withOverwrite, out string _))
        {
            return false;
        }

        // Save to model 
        if (this.quanticsStudioModel.SaveComputerMetadata(
            this.FileInformation, withOverwrite, out string message))
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
                this.FileInformation, withOverwrite, out message);
        this.ValidationMessage = validated ? string.Empty : message;
        return validated;
    }

    public void OnEditing() => this.fileValidator.Validate(this);

    #region Bound  Properties 

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Description { get => this.Get<string>()!; set => this.Set(value); }

    public string ValidationMessage { get => this.Get<string>()!; set => this.Set(value); }

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public ICommand OverwriteCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CancelCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public bool FormIsValid { get => this.Get<bool>(); set => this.Set(value); }

    #endregion Bound Properties 
}
