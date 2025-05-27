namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public partial class ControlledGateView : BehaviorEnabledUserControl, IView
{
    public ControlledGateView()
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