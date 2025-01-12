namespace Lyt.Quantics.Studio.Workflow.Run.Toolbox;

public partial class GatesView : BehaviorEnabledUserControl
{
    public GatesView()
    {
        this.InitializeComponent();
        this.DataContextChanged +=
            (s, e) => new DragOverAble(StageView.HideDropTarget).Attach(this);
    }
}