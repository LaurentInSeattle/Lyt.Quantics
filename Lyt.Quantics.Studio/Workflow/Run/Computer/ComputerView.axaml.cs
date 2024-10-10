namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public partial class ComputerView : UserControl
{
    public ComputerView()
    {
        this.InitializeComponent();
        this.AddHandler(
            DragDrop.DragOverEvent,
            this.OnDragOver,
            RoutingStrategies.Direct | RoutingStrategies.Bubble | RoutingStrategies.Tunnel,
            handledEventsToo: true);
        this.AddHandler(DragDrop.DropEvent, this.OnDrop);
    }

    private void OnDragOver(object? sender, DragEventArgs dragEventArgs)
    {
        // Debug.WriteLine("On Drag Over Gates View");
        dragEventArgs.DragEffects = DragDropEffects.Move;
        var data = dragEventArgs.Data;
        if (data.Get(GateViewModel.CustomDragAndDropFormat) is GateViewModel gateViewModel)
        {
            gateViewModel.View.OnParentDragOver(dragEventArgs);
        }
    }

    private void OnDrop(object? sender, DragEventArgs e)
    {
        Debug.WriteLine("Drop");

        var data = e.Data.Get(GateViewModel.CustomDragAndDropFormat);
        if (data is not GateViewModel gateViewModel)
        {
            Debug.WriteLine("No data");
            return;
        }

        if (this.DataContext is not GatesViewModel gatesViewModel)
        {
            return;
        }

        // gatesViewModel.Drop(gateViewModel);
    }
}