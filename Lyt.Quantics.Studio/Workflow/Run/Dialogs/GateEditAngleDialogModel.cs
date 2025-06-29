﻿namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed partial class GateEditAngleDialogModel
    : DialogViewModel<GateEditAngleDialog, IGateInfoProvider>
{
    private const int DefaultPredefinedValue = 8;

    private static readonly Dictionary<int, AnglePredefinedValue> PredefinedValues =
        new()
        {
            { 0,  new AnglePredefinedValue( 1, false ) },
            { 1,  new AnglePredefinedValue( 2, false ) },
            { 2,  new AnglePredefinedValue( 4, false ) },
            { 3,  new AnglePredefinedValue( 8, false ) },
            { 4,  new AnglePredefinedValue( 16, false ) },

            { 5,  new AnglePredefinedValue( 16,true ) },
            { 6,  new AnglePredefinedValue( 8, true ) },
            { 7,  new AnglePredefinedValue( 4, true ) },
            { 8,  new AnglePredefinedValue( 2, true ) },
            { 9,  new AnglePredefinedValue( 1, true ) },
        };

    [ObservableProperty]
    private string customValue;

    [ObservableProperty]
    private string validationMessage;

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string angleValueText;

    [ObservableProperty]
    private bool saveButtonIsEnabled;

    [ObservableProperty]
    private bool makeControlledButtonIsEnabled;

    [ObservableProperty]
    private double valuesCount;

    [ObservableProperty]
    private double sliderValue;

    private bool isChangedFromSlider;
    private bool isInitializing;

    public GateEditAngleDialogModel()
    {
        this.GateParameters = new();
        this.IsMakeControlled = false;
        this.CustomValue = string.Empty;
        this.ValidationMessage = string.Empty;
        this.AngleValueText = string.Empty;
        this.ValuesCount = GateEditAngleDialogModel.PredefinedValues.Count - 1;
        this.PredefinedValue = GateEditAngleDialogModel.PredefinedValues[DefaultPredefinedValue];
    }

    public IGateInfoProvider GateInfoProvider
        => base.parameters is IGateInfoProvider gVm ?
                gVm :
                throw new ArgumentNullException("No parameters");

    public GateParameters GateParameters { get; private set; }

    public bool IsMakeControlled { get; set; }

    public AnglePredefinedValue PredefinedValue { get; private set; }

    public double AngleValue { get; private set; }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Retrieve GateParameters for the existing gate
        var quanticsStudioModel = App.GetRequiredService<QsModel>();
        var gate = this.GateInfoProvider.Gate;
        int stageIndex = this.GateInfoProvider.StageIndex;
        var stage = quanticsStudioModel.QuComputer.Stages[stageIndex];
        var stageOperator = stage.StageOperatorAt(this.GateInfoProvider.QubitsIndices);
        this.GateParameters = stageOperator.GateParameters;

        this.IsMakeControlled = false;
        this.MakeControlledButtonIsEnabled = gate.IsMutable;

        bool isRotation = false;
        if (gate is RotationGate rotationGate)
        {
            isRotation = true;
            this.GateParameters.Axis = rotationGate.Axis;
        }

        this.Title = isRotation ? "Rotation Angle" : "Phase Value";

        if (this.GateParameters.IsPiDivisor)
        {
            // retrieve the slider value from gate parameters
            this.SliderValue = SliderValueFromParameters(this.GateParameters);
        }
        else
        {
            this.isInitializing = true;
            this.SliderValue = GateEditAngleDialogModel.DefaultPredefinedValue;
            this.AngleValue = this.GateParameters.Angle;
            this.AngleValueText =
                string.Concat(this.Title, ": ", this.AngleValue.ToString("F3"), " radians.");
            this.CustomValue = this.AngleValue.ToString("F3");
        }

        this.ValidationMessage = string.Empty;
        this.SaveButtonIsEnabled = true;
        this.isInitializing = false;
    }

    [RelayCommand]
    public void OnMakeControlled()
    {
        this.IsMakeControlled = true;
        this.onClose?.Invoke(this, true);

        // The delegate should be invoked only once 
        this.onClose = null;

        // We will run another modal dialog, so do NOT dismis the dialog service
    }

    [RelayCommand]
    public void OnSave() => base.TrySaveAndClose();

    [RelayCommand]
    public void OnCancel() => base.Cancel();

    public override bool Validate()
    {
        if (this.GateParameters.IsPiDivisor)
        {
            return true;
        }

        return this.Validate(out string _);
    }

    /// <summary> Called from the view whenever the content of a text box is changed.</summary>
    public void OnEditing()
    {
        if (this.isChangedFromSlider)
        {
            return;
        }

        bool validated = this.Validate(out string message);
        this.ValidationMessage = validated ? string.Empty : message;
        this.SaveButtonIsEnabled = validated;
    }

    private bool Validate(out string message)
    {
        message = string.Empty;
        if (this.IsMakeControlled)
        {
            return true;
        }

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
                this.AngleValue = value;
                this.AngleValueText =
                    string.Concat(this.Title, ": ", value.ToString("F3"), " radians.");

                this.GateParameters.IsPiDivisor = false;
                this.GateParameters.Angle = value;

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

        if (GateEditAngleDialogModel.PredefinedValues.TryGetValue(sliderValue, out var angleValue))
        {
            if (angleValue is AnglePredefinedValue predefinedValue)
            {
                this.isChangedFromSlider = true;
                this.PredefinedValue = predefinedValue;
                this.AngleValue = predefinedValue.Value;
                this.AngleValueText = string.Concat(this.Title, ": ", predefinedValue.Caption);
                this.CustomValue = this.AngleValue.ToString("F3");

                this.GateParameters.IsPiDivisor = true;
                this.GateParameters.Angle = this.AngleValue;
                this.GateParameters.PiDivisor = predefinedValue.PiDivisor;
                this.GateParameters.IsPositive = predefinedValue.IsPositive;

                this.ValidationMessage = string.Empty;
                this.SaveButtonIsEnabled = true;

                // Need to delay a bit for clearing the flag or else the text changed event handler
                // will not 'see' that the isChangedFromSlider flag is set 
                Schedule.OnUiThread(
                    66,
                    () => { this.isChangedFromSlider = false; },
                    DispatcherPriority.Background);
            }
        }
    }

    private static int SliderValueFromParameters(GateParameters gateParameters)
    {
        int piDivisor = gateParameters.PiDivisor;
        bool isPositive = gateParameters.IsPositive;
        foreach (int key in PredefinedValues.Keys)
        {
            var preDefinedValue = PredefinedValues[key];
            if ((preDefinedValue.PiDivisor == piDivisor) &&
                (preDefinedValue.IsPositive == isPositive))
            {
                return key;
            }
        }

        throw new Exception("Failed to setup slider.");
    }

    partial void OnSliderValueChanged(double value)
        => this.OnSliderChanged((int)Math.Round(value));
}
