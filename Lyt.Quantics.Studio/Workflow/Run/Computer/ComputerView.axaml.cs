namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public partial class ComputerView : BehaviorEnabledUserControl
{
    public ComputerView()
    {
        this.InitializeComponent();
        this.DataContextChanged +=
            (s, e) =>
            {
                new DragOverAble(StageView.HideDropTarget).Attach(this);
                new DropAble(StageView.HideDropTarget).Attach(this);
            };
    }
}