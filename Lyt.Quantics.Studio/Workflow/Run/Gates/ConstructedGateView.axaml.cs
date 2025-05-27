namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public partial class ConstructedGateView : BehaviorEnabledUserControl, IView
{
    public ConstructedGateView()
    {
        this.InitializeComponent();
        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
    }
}