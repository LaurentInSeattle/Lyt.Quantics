namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed partial class GateEditControlDialogModel 
    : DialogViewModel<GateEditControlDialog, IGateInfoProvider>
{
    private int controlIndex;
    private int targetIndex;

    [ObservableProperty]
    private string? validationMessage;

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private double valuesCount;

    [ObservableProperty]
    private string? controlValueText;

    [ObservableProperty]
    private string? targetValueText;

    [ObservableProperty]
    private bool saveButtonIsEnabled;

    [ObservableProperty]
    private double controlSliderValue;

    [ObservableProperty]
    private double targetSliderValue;

    public GateEditControlDialogModel() { }

    public IGateInfoProvider GateInfoProvider
        => base.parameters is IGateInfoProvider gVm ?
                gVm :
                throw new ArgumentNullException("No parameters");

    public QubitsIndices QubitsIndices { get; private set; } = new();

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        var gate = this.GateInfoProvider.Gate;
        this.Title = string.Format("Mutate '{0}' to Controlled Gate", gate.CaptionKey);

        // Retrieve GateParameters for the existing gate
        var quanticsStudioModel = App.GetRequiredService<QsModel>();
        int quBitsCount = quanticsStudioModel.QuComputer.QuBitsCount;
        int indexTarget = this.GateInfoProvider.QubitsIndices.TargetQuBitIndices[0];
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

    [RelayCommand]
    public void OnSave()
    {
        this.QubitsIndices = new(this.controlIndex, this.targetIndex);
        this.onClose?.Invoke(this, true);
        this.dialogService.Dismiss();
    }

    [RelayCommand]
    public void OnCancel()
    {
        this.onClose?.Invoke(this, false);
        this.dialogService.Dismiss();
    }

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

    partial void OnControlSliderValueChanged(double value)
        => this.OnSliderChanged((int)Math.Round(value), isControl: true);

    partial void OnTargetSliderValueChanged(double value)
        =>this.OnSliderChanged((int)Math.Round(value), isControl: false);
}
