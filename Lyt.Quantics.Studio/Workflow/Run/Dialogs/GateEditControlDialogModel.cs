namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class GateEditControlDialogModel : DialogBindable<GateEditControlDialog, GateViewModel>
{
    private bool isInitializing;

    public GateEditControlDialogModel()
    {
        this.GateParameters = new();
    }

    public GateViewModel GateViewModel
        => base.parameters is GateViewModel gVm ?
                gVm :
                throw new ArgumentNullException("No parameters");

    public GateParameters GateParameters { get; private set; }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Retrieve GateParameters for the existing gate
        var quanticsStudioModel = App.GetRequiredService<QsModel>();
        var gate = this.GateViewModel.Gate;
        int stageIndex = this.GateViewModel.StageIndex;
        var stage = quanticsStudioModel.QuComputer.Stages[stageIndex];
        var stageOperator = stage.StageOperatorAt(this.GateViewModel.QubitIndex);
        this.GateParameters = stageOperator.GateParameters;

        this.Title = "Controlled Gate" ;

        this.ValidationMessage = string.Empty;
        this.SaveButtonIsEnabled = true;
        this.isInitializing = false;
    }

#pragma warning disable IDE0051 // Remove unused private members

    private void OnSave(object? _)
    {
        this.onClose?.Invoke(this, true);
        this.dialogService.Dismiss();
    }

    private void OnCancel(object? _)
    {
        this.onClose?.Invoke(this, false);
        this.dialogService.Dismiss();
    }
#pragma warning restore IDE0051 // Remove unused private members

    /// <summary> Called from the view whenever the content of a text box is changed.</summary>
    public void OnEditing()
    {
        bool validated = this.Validate(out string message);
        this.ValidationMessage = validated ? string.Empty : message;
        this.SaveButtonIsEnabled = validated;
    }

    private bool Validate(out string message)
    {
        message = string.Empty;
        if (double.TryParse(this.CustomValue, out double value))
        {
            if (value.IsAlmostEqual(0.0))
            {
                message = "Cannot be zero.";
            }
            else if ((value < -Math.PI) || (value > Math.PI))
            {
                message = "Valid range: [ - π , + π ]";
            }
            else
            {
                return true;
            }
        }
        else
        {
            message = "Invalid entry";
        }

        return false;
    }

    private void OnSliderChanged(int sliderValue)
    {
        if (this.isInitializing)
        {
            return;
        }

    }

    public string CustomValue { get => this.Get<string>()!; set => this.Set(value); }

    public string ValidationMessage { get => this.Get<string>()!; set => this.Set(value); }

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CancelCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public double ValuesCount { get => this.Get<double>(); set => this.Set(value); }

    public double SliderValue
    {
        get => this.Get<double>();
        set
        {
            this.Set(value);
            this.OnSliderChanged((int)Math.Round(this.SliderValue));
        }
    }

    public string AngleValueText { get => this.Get<string>()!; set => this.Set(value); }

    public bool SaveButtonIsEnabled
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.View.SaveButton.IsDisabled = !value;
        }
    }
}
