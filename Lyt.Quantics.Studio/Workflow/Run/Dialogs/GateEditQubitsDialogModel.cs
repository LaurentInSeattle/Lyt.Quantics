namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed partial class GateEditQubitsDialogModel
    : DialogViewModel<GateEditQubitsDialog, IGateInfoProvider>
{
    public sealed record class QubitSetup(int Index, bool IsControl);

    [ObservableProperty] private bool hasThreeQubits;
    [ObservableProperty] private string? validationMessage;
    [ObservableProperty] private string? title;
    [ObservableProperty] private double valuesCount;
    [ObservableProperty] private double firstSliderValue;
    [ObservableProperty] private string? firstValueTextLabel;
    [ObservableProperty] private string? firstValueText;
    [ObservableProperty] private double secondSliderValue;
    [ObservableProperty] private string? secondValueTextLabel;
    [ObservableProperty] private string? secondValueText;
    [ObservableProperty] private double thirdSliderValue;
    [ObservableProperty] private string? thirdValueTextLabel;
    [ObservableProperty] private string? thirdValueText;
    [ObservableProperty] private bool saveButtonIsEnabled;

    private int firstIndex;
    private int secondIndex;
    private int thirdIndex;
    private QuStageOperator stageOperator;

    public GateEditQubitsDialogModel() => this.stageOperator = new();

    public IGateInfoProvider GateInfoProvider
        => base.parameters is IGateInfoProvider gVm ?
                gVm :
                throw new ArgumentNullException("No parameters");

    public QubitsIndices QubitsIndices { get; private set; } = new();

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Retrieve various parameters and characteristics for the existing gate
        var quanticsStudioModel = App.GetRequiredService<QsModel>();
        int quBitsCount = quanticsStudioModel.QuComputer.QuBitsCount;
        var gate = this.GateInfoProvider.Gate;
        int stageIndex = this.GateInfoProvider.StageIndex;
        var stage = quanticsStudioModel.QuComputer.Stages[stageIndex];
        this.stageOperator = stage.StageOperatorAt(this.GateInfoProvider.QubitsIndices);
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

    [RelayCommand]
    public void OnSave()
    {
        this.PopulateStageParameters();
        this.onClose?.Invoke(this, true);
        this.dialogService.Dismiss();
    }

    [RelayCommand]
    public void OnCancel()
    {
        this.onClose?.Invoke(this, false);
        this.dialogService.Dismiss();
    }

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
        this.QubitsIndices.Clear();
        var controlIndices = this.QubitsIndices.ControlQuBitIndices;
        var targetIndices = this.QubitsIndices.TargetQuBitIndices;
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
        bool fail;
        if (this.HasThreeQubits)
        {
            fail =
                (this.firstIndex == this.secondIndex) ||
                (this.firstIndex == this.thirdIndex) ||
                (this.secondIndex == this.thirdIndex);
        }
        else
        {
            fail = this.firstIndex == this.secondIndex;
        }

        if (fail)
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

    partial void OnFirstSliderValueChanged(double value)
        => this.OnSliderChanged((int)Math.Round(value), sliderIndex: 0);

    partial void OnSecondSliderValueChanged(double value)
        => this.OnSliderChanged((int)Math.Round(value), sliderIndex: 1);

    partial void OnThirdSliderValueChanged(double value)
        => this.OnSliderChanged((int)Math.Round(value), sliderIndex: 2);
}
