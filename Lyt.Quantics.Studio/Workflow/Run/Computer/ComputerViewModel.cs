namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public sealed class ComputerViewModel : Bindable<ComputerView>
{
    public bool CanDrop(Point point)
    {
        // TODO
        return point.X > point.Y ; 
    }

    public void OnDrop (Point point, GateViewModel gateViewModel)
    {
        Debug.WriteLine("ComputerViewModel: OnDrop");
    }
}
