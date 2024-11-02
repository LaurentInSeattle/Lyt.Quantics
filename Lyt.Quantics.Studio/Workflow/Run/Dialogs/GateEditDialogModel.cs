namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class GateEditDialogModel : DialogBindable<GateEditDialog, GateViewModel> 
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

    public GateEditDialogModel()
    {
        this.IsPredefinedValue = true;
        this.PredefinedValue = GateEditDialogModel.PredefinedValues[DefaultPredefinedValue]; 
    }

    private GateViewModel GateViewModel 
        => base.parameters is GateViewModel gVm ? 
                gVm : 
                throw new ArgumentNullException("No parameters"); 

    public bool IsPredefinedValue {  get; private set; }

    public AnglePredefinedValue PredefinedValue { get; private set; }

    public double AngleValue { get; private set; }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        this.Title =
            this.GateViewModel.Gate.CaptionKey.StartsWith('R') ?
                "Gate Rotation Angle" :
                "Gate Phase Value";
        this.ValuesCount = GateEditDialogModel.PredefinedValues.Count-1;
        this.SliderValue = GateEditDialogModel.DefaultPredefinedValue;
    }

    private void OnSave(object? _)
    {
        this.onClose?.Invoke(this, true);
        this.dialogService.Dismiss();
    }

    public void OnCancel(object? _)
    {
        this.onClose?.Invoke(this, false);
        this.dialogService.Dismiss();
    }

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
            int sliderValue = (int)Math.Round(this.SliderValue);
            
            if (GateEditDialogModel.PredefinedValues.TryGetValue(sliderValue, out var angleValue))
            {
                if (angleValue is AnglePredefinedValue predefinedValue)
                {
                    this.IsPredefinedValue = true;
                    this.PredefinedValue = predefinedValue;
                    this.AngleValue = predefinedValue.Value;
                    this.AngleValueText = string.Concat( this.Title , ": " , predefinedValue.Caption);
                } 
            }
        }
    }

    public string AngleValueText { get => this.Get<string>()!; set => this.Set(value); }
}
