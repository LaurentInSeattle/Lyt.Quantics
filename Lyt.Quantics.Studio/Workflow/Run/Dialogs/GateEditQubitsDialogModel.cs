namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class GateEditQubitsDialogModel : DialogBindable<GateEditQubitsDialog, GateViewModel>
{
    public sealed record class QubitSetup(int Index, bool IsControl);

    private int firstIndex;
    private int secondIndex;
    private int thirdIndex;
    private QuStageOperator stageOperator;

    public GateEditQubitsDialogModel() => this.stageOperator = new();

    public GateViewModel GateViewModel
        => base.parameters is GateViewModel gVm ?
                gVm :
                throw new ArgumentNullException("No parameters");

    public StageOperatorParameters StageOperatorParameters { get; private set; } = new();

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Retrieve various parameters and characteristics for the existing gate
        var quanticsStudioModel = App.GetRequiredService<QsModel>();
        int quBitsCount = quanticsStudioModel.QuComputer.QuBitsCount;
        var gate = this.GateViewModel.Gate;
        int stageIndex = this.GateViewModel.StageIndex;
        var stage = quanticsStudioModel.QuComputer.Stages[stageIndex];
        this.stageOperator = stage.StageOperatorAt(this.GateViewModel.QubitIndex);
        this.HasThreeQubits = gate.QuBitsTransformed == 3;
        this.Title = gate.CaptionKey + ": Edit Qubits Inputs";

        // Force property changed 
        this.SaveButtonIsEnabled = true;
        this.SaveButtonIsEnabled = false;
        this.ValidationMessage = string.Empty;
        this.ValuesCount = quBitsCount - 1;

        this.SetupLabels();
        this.SetupSliders();

        this.ValidationMessage = string.Empty;
        this.SaveButtonIsEnabled = true;
        this.Validate(out string _);
    }

#pragma warning disable IDE0051 // Remove unused private members

    private void OnSave(object? _)
    {
        this.PopulateStageParameters();
        this.onClose?.Invoke(this, true);
        this.dialogService.Dismiss();
    }

    private void OnCancel(object? _)
    {
        this.onClose?.Invoke(this, false);
        this.dialogService.Dismiss();
    }
#pragma warning restore IDE0051 // Remove unused private members

    private void SetupLabels()
    {
        int controls = this.stageOperator.ControlQuBitIndices.Count;
        int targets = this.stageOperator.TargetQuBitIndices.Count;
        if ((controls == 1) && (targets == 1))
        {
            this.FirstValueTextLabel = "Control Qubit Index:";
            this.SecondValueTextLabel = "Target Qubit Index:";
        }
        else if ((controls == 0) && (targets == 2))
        {
            this.FirstValueTextLabel = "First Target Qubit Index:";
            this.SecondValueTextLabel = "Second Target Qubit Index:";
        }
        else if ((controls == 1) && (targets == 2))
        {
            this.FirstValueTextLabel = "Control Qubit Index:";
            this.SecondValueTextLabel = "First Target Qubit Index:";
            this.ThirdValueTextLabel = "Second Target Qubit Index:";
        }
        else if ((controls == 2) && (targets == 1))
        {
            this.FirstValueTextLabel = "First Control Qubit Index:";
            this.SecondValueTextLabel = "Second Control Qubit Index:";
            this.ThirdValueTextLabel = "Target Qubit Index:";
        }
        else if ((controls == 0) && (targets == 3))
        {
            this.FirstValueTextLabel = "First Target Qubit Index:";
            this.SecondValueTextLabel = "Second Target Qubit Index:";
            this.ThirdValueTextLabel = "Third Target Qubit Index:";
        }
        else
        {
            throw new Exception("No such combination of controls and targets.");
        }
    }

    private void SetupSliders()
    {
        this.FirstSliderValue = 0;
        this.SecondSliderValue = 0;
        this.ThirdSliderValue = 0;

        var controlQubits = this.stageOperator.ControlQuBitIndices;
        var targetQubits = this.stageOperator.TargetQuBitIndices;
        int controls = controlQubits.Count;
        int targets = targetQubits.Count;
        if ((controls == 1) && (targets == 1))
        {
            this.FirstSliderValue = controlQubits[0];
            this.SecondSliderValue = targetQubits[0];
        }
        else if ((controls == 0) && (targets == 2))
        {
            this.FirstSliderValue = targetQubits[0];
            this.SecondSliderValue = targetQubits[1];
        }
        else if ((controls == 1) && (targets == 2))
        {
            this.FirstSliderValue = controlQubits[0];
            this.SecondSliderValue = targetQubits[0];
            this.ThirdSliderValue = targetQubits[1];
        }
        else if ((controls == 2) && (targets == 1))
        {
            this.FirstSliderValue = controlQubits[0];
            this.SecondSliderValue = controlQubits[1];
            this.ThirdSliderValue = targetQubits[0];
        }
        else if ((controls == 0) && (targets == 3))
        {
            this.FirstSliderValue = targetQubits[0];
            this.SecondSliderValue = targetQubits[1];
            this.ThirdSliderValue = targetQubits[2];
        }
        else
        {
            throw new Exception("No such combination of controls and targets.");
        }
    }

    private void PopulateStageParameters()
    {
        int controls = this.stageOperator.ControlQuBitIndices.Count;
        int targets = this.stageOperator.TargetQuBitIndices.Count;
        this.StageOperatorParameters.Clear();
        var controlIndices = this.StageOperatorParameters.TargetQuBitIndices;
        var targetIndices = this.StageOperatorParameters.TargetQuBitIndices;
        if ((controls == 1) && (targets == 1))
        {
            controlIndices.Add(firstIndex);
            targetIndices.Add(secondIndex);
        }
        else if ((controls == 0) && (targets == 2))
        {
            targetIndices.Add(firstIndex);
            targetIndices.Add(secondIndex);
        }
        else if ((controls == 1) && (targets == 2))
        {
            controlIndices.Add(firstIndex);
            targetIndices.Add(secondIndex);
            targetIndices.Add(thirdIndex);
        }
        else if ((controls == 2) && (targets == 1))
        {
            controlIndices.Add(firstIndex);
            controlIndices.Add(secondIndex);
            targetIndices.Add(thirdIndex);
        }
        else if ((controls == 0) && (targets == 3))
        {
            targetIndices.Add(firstIndex);
            targetIndices.Add(secondIndex);
            targetIndices.Add(thirdIndex);
        }
        else
        {
            throw new Exception("No such combination of controls and targets.");
        }
    }

    private bool Validate(out string message)
    {
        bool validated = true;
        message = string.Empty;
        if ((this.firstIndex == this.secondIndex) ||
            (this.firstIndex == this.thirdIndex) ||
            (this.secondIndex == this.thirdIndex))
        {
            message = "All qubit indices must be distinct.";
            validated = false;
        }

        this.SaveButtonIsEnabled = validated;
        this.ValidationMessage = validated ? string.Empty : message;
        return validated;
    }

    private void OnSliderChanged(int sliderValue, int sliderIndex)
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

    public bool HasThreeQubits { get => this.Get<bool>(); set => this.Set(value); }

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
