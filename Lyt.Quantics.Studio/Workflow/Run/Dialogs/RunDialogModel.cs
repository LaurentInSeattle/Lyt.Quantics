namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed partial class RunDialogModel : DialogViewModel<RunDialog, object>, IRecipient<ModelProgressMessage>
{
    private readonly QsModel quanticsStudioModel;

    [ObservableProperty]
    public partial float ProgressTotal { get; set; }

    [ObservableProperty]
    public partial float ProgressValue { get; set; }

    [ObservableProperty]
    public partial string? Message { get; set; }

    [ObservableProperty]
    public partial string? Title { get; set; }

    [ObservableProperty]
    public partial bool RingIsActive { get; set; }

    public RunDialogModel()
    {
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
        this.Title = "Calculations in Progress...";
        this.CanEnter = false;
        this.CanEscape = false;
        this.RingIsActive = true;
        this.ProgressValue = 0;
        this.ProgressTotal = this.quanticsStudioModel.QuComputer.Stages.Count;
        this.Message = "Calculation started...";

        this.Subscribe<ModelProgressMessage>();
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        _ = this.quanticsStudioModel.Run(runUsingKroneckerProduct: false, runAsync: true);
    }

    public void Receive(ModelProgressMessage message)
    {
        Dispatch.OnUiThread(() =>
        {
            if (message.IsComplete)
            {
                new ModelResultsUpdateMessage().Publish();
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
        });
    }

    [RelayCommand]
    public async Task OnCancel()
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
}
