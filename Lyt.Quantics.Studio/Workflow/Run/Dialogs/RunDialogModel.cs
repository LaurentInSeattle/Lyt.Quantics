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
    }

    protected override void OnViewLoaded()
    { 
        base.OnViewLoaded();
        this.RingIsActive = true;
        _ = Task.Run (this.Run);
    }

    private void Run ()
    {
        _ = this.quanticsStudioModel.Run(runUsingKroneckerProduct: false);
        Dispatch.OnUiThread (() =>
        {
            this.RingIsActive = false;
            this.Cancel();
        }, DispatcherPriority.Normal);
    }

#pragma warning disable IDE0051 // Remove unused private members

    private void OnCancel(object? _) => this.Cancel();

#pragma warning restore IDE0051 // Remove unused private members

    #region Bound  Properties 

    public string Message { get => this.Get<string>()!; set => this.Set(value); }

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public bool RingIsActive { get => this.Get<bool>(); set => this.Set(value); }

    public ICommand CancelCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    #endregion Bound Properties 
}
