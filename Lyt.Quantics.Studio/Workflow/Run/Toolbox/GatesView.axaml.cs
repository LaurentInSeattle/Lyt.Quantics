namespace Lyt.Quantics.Studio.Workflow.Run.Toolbox;

public partial class GatesView : View
{
    protected override void OnDataContextChanged(object? sender, EventArgs e)
    {
        base.OnDataContextChanged(sender, e);
        new DragOverAble(StageView.HideDropTarget).Attach(this);
    }
}