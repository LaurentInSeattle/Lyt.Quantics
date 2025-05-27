namespace Lyt.Quantics.Studio.Workflow.Run.Toolbox;

public partial class GatesView : BehaviorEnabledUserControl, IView
{
    public GatesView()
    {
        this.InitializeComponent();
        this.DataContextChanged +=
            (s, e) => new DragOverAble(StageView.HideDropTarget).Attach(this);
        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
    }
}