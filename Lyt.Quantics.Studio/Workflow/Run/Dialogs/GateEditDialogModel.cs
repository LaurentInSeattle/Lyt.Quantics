using static Lyt.Quantics.Studio.Messaging.ToolbarCommandMessage;

namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class GateEditDialogModel : DialogBindable<GateEditDialog, GateViewModel> 
{
    public GateEditDialogModel()
    {
    }

    private GateViewModel GateViewModel 
        => base.parameters is GateViewModel gVm ? 
                gVm : 
                throw new ArgumentNullException("No parameters"); 

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.ValuesCount = 10;
        this.Title =
            this.GateViewModel.Gate.CaptionKey.StartsWith('R') ?
                "Gate Rotation Angle" :
                "Gate Phase Value"; 
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
            this.AngleValueText = string.Format("Angle: {0:D}", sliderValue);
        }
    }

    public string AngleValueText { get => this.Get<string>()!; set => this.Set(value); }
}
