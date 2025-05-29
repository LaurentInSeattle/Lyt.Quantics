namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

using static SharedValidators;

public sealed partial class SaveDialogModel : DialogViewModel<SaveDialog, object>
{
    private readonly QsModel quanticsStudioModel;
    private readonly FormValidator<FileInformation> fileValidator;

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string? name;

    [ObservableProperty]
    private string? description;

    [ObservableProperty]
    private bool formIsValid;

    [ObservableProperty]
    private string? validationMessage;

    public SaveDialogModel()
    {
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
        this.Title = "Save your Project";
        this.fileValidator =
            new FormValidator<FileInformation>(
                new(FormValidPropertyName: "FormIsValid",
                    FocusFieldName: "NameTextBox",
                    FieldValidators: [NameValidator, DescriptionValidator]));
        this.CanEnter = false;
        this.ClearForm();
    }

    //public override void OnViewLoaded()
    //{
    //    base.OnViewLoaded();
    //    this.ClearForm();
    //}

    private void ClearForm()
    {
        this.fileValidator.Clear(this);
        var computer = this.quanticsStudioModel.QuComputer;
        this.Name = computer.Name;
        this.Description = computer.Description;
        this.fileValidator.Validate(this);
    }

    [RelayCommand]
    public void OnOverwrite()
    {
        if (this.TrySave(withOverwrite: true))
        {
            this.TrySaveAndClose();
        }
    }

    [RelayCommand]
    public void OnSave()
    {
        if (this.TrySave(withOverwrite: false))
        {
            this.TrySaveAndClose();
        }
    }

    [RelayCommand]
    public void OnCancel() => this.Cancel();

    private bool TrySave(bool withOverwrite)
    {
        // Extra validation when about to save 
        if (!this.Validate(withOverwrite, out string _))
        {
            return false;
        }

        // Save to model 
        if (this.quanticsStudioModel.SaveComputerMetadata(
            this.fileValidator.Value, withOverwrite, out string message))
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
                this.fileValidator.Value, withOverwrite, out message);
        this.ValidationMessage = validated ? string.Empty : message;
        return validated;
    }

    public void OnEditing() => this.fileValidator.Validate(this);
}
