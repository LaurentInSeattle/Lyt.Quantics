namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

public partial class AmplitudesView : BehaviorEnabledUserControl, IView
{
    public AmplitudesView()
    {
        this.InitializeComponent();
        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
        this.DataContextChanged +=
            (s, e) => new DragOverAble(StageView.HideDropTarget).Attach(this);
    }
}