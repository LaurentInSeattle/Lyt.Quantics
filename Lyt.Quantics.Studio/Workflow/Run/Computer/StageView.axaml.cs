namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public partial class StageView : BehaviorEnabledUserControl
{
    public static readonly DropTargetView DropTargetView;

    private static StageView? dropTargetViewOwner;

    static StageView() => DropTargetView = new DropTargetView();

    public static void HideDropTarget()
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

    public static bool ShowDropTarget(IDropTarget dropTarget, Point position)
    {
        if ((dropTarget is not StageViewModel stageViewModel) || !stageViewModel.IsBound)
        {
            return false;
        } 

        var stageView = stageViewModel.View;
        if (dropTargetViewOwner is not null && dropTargetViewOwner != stageView)
        {
            HideDropTarget();
        }

        stageViewModel.ShowDropTarget(DropTargetView, position);
        dropTargetViewOwner = stageView;

        return true;
    }

    public StageView()
    {
        this.InitializeComponent();

        this.DataContextChanged +=
            (s, e) =>
            {
                if (this.DataContext is StageViewModel stageViewModel)
                {
                    stageViewModel.BindOnDataContextChanged(this);
                }

                new DragOverAble(StageView.HideDropTarget, StageView.ShowDropTarget).Attach(this);
                new DropAble(StageView.HideDropTarget).Attach(this);
            };
    }
}