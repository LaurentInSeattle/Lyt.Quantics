namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

public partial class AmplitudesView : BehaviorEnabledUserControl
{
    public AmplitudesView()
    {
        this.InitializeComponent();
        this.DataContextChanged += 
            (s,e)=> new DragOverAble(StageView.HideDropTarget).Attach(this);
    }
}