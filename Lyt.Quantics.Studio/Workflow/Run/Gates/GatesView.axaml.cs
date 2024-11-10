namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public partial class GatesView : UserControl
{
    public GatesView()
    {
        this.InitializeComponent();
        DragDrop.SetAllowDrop(this, true);
        this.AddHandler(DragDrop.DragOverEvent, this.OnDragOver);
    }

    private void OnDragOver(object? sender, DragEventArgs dragEventArgs)
    {
        var data = dragEventArgs.Data;
        dragEventArgs.DragEffects = DragDropEffects.None;
        if (data.Get(GateViewModel.CustomDragAndDropFormat) is IDraggableBindable draggableBindable)
        {
            var draggable = draggableBindable.Draggable;
            draggable?.OnParentDragOver(dragEventArgs);
        }
    }
}