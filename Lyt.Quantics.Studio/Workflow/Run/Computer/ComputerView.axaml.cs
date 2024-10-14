namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public partial class ComputerView : UserControl
{
    public ComputerView()
    {
        this.InitializeComponent();

        this.SetValue(DragDrop.AllowDropProperty, true);
        this.AddHandler(DragDrop.DragOverEvent, this.OnDragOver);
        this.AddHandler(DragDrop.DropEvent, this.OnDrop);
    }

    private void OnDragOver(object? sender, DragEventArgs dragEventArgs)
    {
        // Debug.WriteLine("On Drag Over Gate Computer View");
        dragEventArgs.DragEffects = DragDropEffects.None;
        var data = dragEventArgs.Data;
        if (data.Get(GateViewModel.CustomDragAndDropFormat) is GateViewModel gateViewModel)
        {
            gateViewModel.View.OnParentDragOver(dragEventArgs);
            if (this.DataContext is ComputerViewModel computerViewModel)
            {
                if (computerViewModel.CanDrop(dragEventArgs.GetPosition(this), gateViewModel))
                {
                    dragEventArgs.DragEffects = DragDropEffects.Move;
                }
            }
        }
    }

    private void OnDrop(object? sender, DragEventArgs dragEventArgs)
    {
        var data = dragEventArgs.Data.Get(GateViewModel.CustomDragAndDropFormat);
        if (data is GateViewModel gateViewModel)
        {
            if (this.DataContext is ComputerViewModel computerViewModel)
            {
                computerViewModel.OnDrop(dragEventArgs.GetPosition(this), gateViewModel);
            }
        }
    }
}