namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed class GateEditDialogModel : DialogBindable<GateEditDialog, GateViewModel> 
{
    private void OnAction(object? _)
    {
        this.onClose?.Invoke(this, true);
        this.dialogService.Dismiss();
    }

    public void OnDismiss(object? _)
    {
        this.onClose?.Invoke(this, false);
        this.dialogService.Dismiss();
    }
}
