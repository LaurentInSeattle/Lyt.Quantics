namespace Lyt.Quantics.Studio.Workflow.Run.Code;

public partial class CodeView : View
{
    protected override void OnDataContextChanged(object? sender, EventArgs e)
    {
        base.OnDataContextChanged(sender, e);
        new DragOverAble(StageView.HideDropTarget).Attach(this);
    }
}