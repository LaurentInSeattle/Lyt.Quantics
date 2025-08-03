
namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public partial class StageView : View
{
    public static readonly DropTargetControl DropTargetView;

    private static StageView? dropTargetViewOwner;

    static StageView() => DropTargetView = new DropTargetControl();

    public static void HideDropTarget(IDropTarget? dropTarget)
    {
        if (dropTargetViewOwner is not null)
        {
            if (dropTargetViewOwner.DataContext is StageViewModel stageViewModel)
            {
                stageViewModel.HideDropTarget(DropTargetView);
            }
        }

        dropTargetViewOwner = null;
    }

    public static bool ShowDropTarget(IDropTarget? dropTarget, Point position)
    {
        if ((dropTarget is not StageViewModel stageViewModel) || !stageViewModel.IsBound)
        {
            return false;
        } 

        var stageView = stageViewModel.View;
        if (dropTargetViewOwner is not null && dropTargetViewOwner != stageView)
        {
            HideDropTarget(dropTarget);
        }

        stageViewModel.ShowDropTarget(DropTargetView, position);
        dropTargetViewOwner = stageView;

        return true;
    }

    protected override void OnDataContextChanged(object? sender, EventArgs e)
    {
        if (this.DataContext is StageViewModel stageViewModel)
        {
            base.OnDataContextChanged(sender, e);
        }

        new DragOverAble(StageView.HideDropTarget, StageView.ShowDropTarget).Attach(this);
        new DropAble(StageView.HideDropTarget).Attach(this);
    }
}