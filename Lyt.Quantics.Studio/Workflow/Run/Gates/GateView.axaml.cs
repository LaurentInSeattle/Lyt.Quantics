namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public partial class GateView : BehaviorEnabledUserControl, IView
{
    public GateView()
    {
        this.InitializeComponent();

        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };

        // Late binding only for the members of the collection backing the ItemsControl 
        this.DataContextChanged += this.OnGateViewDataContextChanged;
    }

    private void OnGateViewDataContextChanged(object? sender, EventArgs e)
    {
        if (this.DataContext is GateViewModel gateViewModel)
        {
            if (!gateViewModel.IsGhost)
            {
                // Don't do that for the ghost view 
                gateViewModel.BindOnDataContextChanged(this);
            }
        }
    }
}