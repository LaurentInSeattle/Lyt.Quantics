namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public partial class GateView : BehaviorEnabledUserControl
{
    public GateView()
    {
        this.InitializeComponent();

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