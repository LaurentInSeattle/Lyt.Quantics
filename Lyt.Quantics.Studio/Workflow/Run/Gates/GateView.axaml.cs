namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public partial class GateView : View
{
    protected override void OnDataContextChanged(object? sender, EventArgs e) 
    {
        if (this.DataContext is GateViewModel gateViewModel)
        {
            if (!gateViewModel.IsGhost)
            {
                // Don't do that for the ghost view 
                base.OnDataContextChanged(sender, e);
            }
        }
    }
}