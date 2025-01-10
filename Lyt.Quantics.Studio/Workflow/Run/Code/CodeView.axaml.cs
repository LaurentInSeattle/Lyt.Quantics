namespace Lyt.Quantics.Studio.Workflow.Run.Code;

public partial class CodeView : BehaviorEnabledUserControl
{
    public CodeView()
    {
        this.InitializeComponent();
        this.DataContextChanged +=
            (s, e) => new DragOverAble(StageView.HideDropTarget).Attach(this);
    }
}