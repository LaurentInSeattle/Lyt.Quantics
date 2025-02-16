using System.Threading.Tasks;

namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class RunDialogModel : DialogBindable<RunDialog, object>
{
    private readonly QsModel quanticsStudioModel;

    public RunDialogModel()
    {
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
        this.Title = "Calculations in Progress...";
        this.CanEnter = false;
        this.CanEscape = false;
        this.Messenger.Subscribe<ModelProgressMessage>(this.OnModelProgress, withUiDispatch: true);
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.RingIsActive = true;
        this.ProgressValue = 0;
        this.ProgressTotal = this.quanticsStudioModel.QuComputer.Stages.Count;
        this.Message = "Calculation started...";

        _ = this.quanticsStudioModel.Run(runUsingKroneckerProduct: false, runAsync: true);
    }

    private void OnModelProgress(ModelProgressMessage message)
    {
        if (message.IsComplete)
        {
            var toaster = App.GetRequiredService<IToaster>();
            toaster.Show("Complete!", "Successful single Run! ", 4_000, InformationLevel.Success);
            this.RingIsActive = false;
            this.Cancel();

        }
        else
        {
            this.Message =
                string.Format(
                    " {0} / {1} calculation steps complete.",
                    message.Step,
                    this.quanticsStudioModel.QuComputer.Stages.Count);
            this.ProgressValue = message.Step;
        } 
    }

#pragma warning disable IDE0051 // Remove unused private members

    private async void OnCancel(object? _)
    {
        var result = await this.quanticsStudioModel.Break();
        if ( !result.Item1)
        {
            var toaster = App.GetRequiredService<IToaster>();
            toaster.Show("Error!", result.Item2, 30_000, InformationLevel.Error); 
        }

        // Cancel even if error 
        this.Cancel();
        this.RingIsActive = false;
    }

#pragma warning restore IDE0051 // Remove unused private members

    #region Bound  Properties 

    public float ProgressTotal { get => this.Get<float>(); set => this.Set(value); }

    public float ProgressValue { get => this.Get<float>(); [DoNotLog] set => this.Set(value); }

    public string Message { get => this.Get<string>()!; set => this.Set(value); }

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public bool RingIsActive { get => this.Get<bool>(); set => this.Set(value); }

    public ICommand CancelCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    #endregion Bound Properties 
}
