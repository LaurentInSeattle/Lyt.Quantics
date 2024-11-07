namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class GateEditQubitsDialogModel : DialogBindable<GateEditQubitsDialog, GateViewModel>
{
    private int firstIndex;
    private int secondIndex;
    private int thirdIndex;

    public GateEditQubitsDialogModel()
    {
    }

    public GateViewModel GateViewModel
        => base.parameters is GateViewModel gVm ?
                gVm :
                throw new ArgumentNullException("No parameters");

    public StageOperatorParameters StageOperatorParameters { get; private set; } = new();

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Retrieve GateParameters for the existing gate
        var quanticsStudioModel = App.GetRequiredService<QsModel>();
        int quBitsCount = quanticsStudioModel.QuComputer.QuBitsCount;
        var gate = this.GateViewModel.Gate;
        int stageIndex = this.GateViewModel.StageIndex;
        var stage = quanticsStudioModel.QuComputer.Stages[stageIndex];
        var stageOperator = stage.StageOperatorAt(this.GateViewModel.QubitIndex);

        this.Title = "Edit Qubits Inputs" ;

        // Force property changed 
        this.SaveButtonIsEnabled = true;
        this.SaveButtonIsEnabled = false;
        this.ValidationMessage = string.Empty;
        this.ValuesCount = quBitsCount - 1;

        this.FirstSliderValue = 0;
        this.SecondSliderValue = 0;
        this.ThirdSliderValue = 0;

        this.FirstValueTextLabel = "First";
        this.SecondValueTextLabel = "Second";
        this.ThirdValueTextLabel = "Third";

        this.ValidationMessage = string.Empty;
        this.SaveButtonIsEnabled = true;
        this.Validate(out string _);
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

    private bool Validate(out string message)
    {
        bool validated = true;
        message = string.Empty;
        if ((this.firstIndex == this.secondIndex)||
            (this.firstIndex == this.thirdIndex)||
            (this.secondIndex == this.thirdIndex))
        {
            message = "All qubit indices must be distinct.";
            validated = false;
        }

        this.SaveButtonIsEnabled = validated;
        this.ValidationMessage = validated ? string.Empty : message;
        return validated;
    }

    private void OnSliderChanged( int sliderValue, int sliderIndex)
    {
        if (sliderIndex == 0)
        {
            this.firstIndex = sliderValue;
            this.FirstValueText = string.Format("{0}", sliderValue);
        }
        else if (sliderIndex == 1)
        {
            this.secondIndex = sliderValue;
            this.SecondValueText = string.Format("{0}", sliderValue);
        }
        else
        {
            this.thirdIndex = sliderValue;
            this.ThirdValueText = string.Format("{0}", sliderValue);
        }

        this.Validate(out string _);
    }

    #region Bound  Properties 

    public string ValidationMessage { get => this.Get<string>()!; set => this.Set(value); }

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CancelCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public double ValuesCount { get => this.Get<double>(); set => this.Set(value); }

    public double FirstSliderValue
    {
        get => this.Get<double>();
        set
        {
            this.Set(value);
            this.OnSliderChanged((int)Math.Round(this.FirstSliderValue), sliderIndex: 0);
        }
    }

    public string FirstValueTextLabel { get => this.Get<string>()!; set => this.Set(value); }

    public string FirstValueText { get => this.Get<string>()!; set => this.Set(value); }

    public double SecondSliderValue
    {
        get => this.Get<double>();
        set
        {
            this.Set(value);
            this.OnSliderChanged((int)Math.Round(this.SecondSliderValue), sliderIndex: 1);
        }
    }

    public string SecondValueTextLabel { get => this.Get<string>()!; set => this.Set(value); }

    public string SecondValueText { get => this.Get<string>()!; set => this.Set(value); }

    public double ThirdSliderValue
    {
        get => this.Get<double>();
        set
        {
            this.Set(value);
            this.OnSliderChanged((int)Math.Round(this.ThirdSliderValue), sliderIndex: 2);
        }
    }

    public string ThirdValueTextLabel { get => this.Get<string>()!; set => this.Set(value); }

    public string ThirdValueText { get => this.Get<string>()!; set => this.Set(value); }

    public bool SaveButtonIsEnabled
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.View.SaveButton.IsDisabled = !value;
        }
    }

    #endregion Bound Properties 
}
