namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class GateEditControlDialogModel : DialogBindable<GateEditControlDialog, GateViewModel>
{
    private int controlIndex;
    private int targetIndex;

    public GateEditControlDialogModel() { }

    public GateViewModel GateViewModel
        => base.parameters is GateViewModel gVm ?
                gVm :
                throw new ArgumentNullException("No parameters");

    public StageOperatorParameters StageOperatorParameters { get; private set; } = new();

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        var gate = this.GateViewModel.Gate;
        this.Title = string.Format("Mutate {0} to Controlled Gate", gate.CaptionKey) ;

        // Retrieve GateParameters for the existing gate
        var quanticsStudioModel = App.GetRequiredService<QsModel>();
        int quBitsCount = quanticsStudioModel.QuComputer.QuBitsCount;
        int indexTarget = this.GateViewModel.QubitIndex;
        int indexBefore = indexTarget - 1;
        int indexAfter = indexTarget + 1;
        int indexControl =
            indexBefore >= 0 ?
                indexBefore :
                indexAfter <= quBitsCount - 1 ? indexAfter : -1;
        if (indexControl == -1)
        {
            // throw ? 
            if (Debugger.IsAttached) { Debugger.Break(); }
        }

        // Force property changed 
        this.SaveButtonIsEnabled = true;
        this.SaveButtonIsEnabled = false;
        this.ValidationMessage = string.Empty;
        this.ValuesCount = quBitsCount - 1;
        this.TargetSliderValue = indexTarget;
        this.ControlSliderValue = indexControl;
    }

#pragma warning disable IDE0051 // Remove unused private members

    private void OnSave(object? _)
    {
        this.StageOperatorParameters = new(this.controlIndex, this.targetIndex);
        this.onClose?.Invoke(this, true);
        this.dialogService.Dismiss();
    }

    private void OnCancel(object? _)
    {
        this.onClose?.Invoke(this, false);
        this.dialogService.Dismiss();
    }
#pragma warning restore IDE0051 // Remove unused private members

    private bool Validate(out string message)
    {
        bool validated = true;
        message = string.Empty;
        if (this.targetIndex == this.controlIndex)
        {
            message = "Control and Target qubit indices must be distinct.";
            validated = false;
        }

        this.SaveButtonIsEnabled = validated;
        this.ValidationMessage = validated ? string.Empty : message;
        return validated;
    }

    private void OnSliderChanged(int sliderValue, bool isControl)
    {
        if (isControl)
        {
            this.controlIndex = sliderValue;
            this.ControlValueText = string.Format("{0}", sliderValue);
        }
        else
        {
            this.targetIndex = sliderValue;
            this.TargetValueText = string.Format("{0}", sliderValue);
        }

        this.Validate(out string _);
    }

    public string ValidationMessage { get => this.Get<string>()!; set => this.Set(value); }

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CancelCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public double ValuesCount { get => this.Get<double>(); set => this.Set(value); }

    public double ControlSliderValue
    {
        get => this.Get<double>();
        set
        {
            this.Set(value);
            this.OnSliderChanged((int)Math.Round(this.ControlSliderValue), isControl: true);
        }
    }

    public double TargetSliderValue
    {
        get => this.Get<double>();
        set
        {
            this.Set(value);
            this.OnSliderChanged((int)Math.Round(this.TargetSliderValue), isControl: false);
        }
    }

    public string ControlValueText { get => this.Get<string>()!; set => this.Set(value); }

    public string TargetValueText { get => this.Get<string>()!; set => this.Set(value); }

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
