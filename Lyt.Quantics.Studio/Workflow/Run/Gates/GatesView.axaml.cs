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
        if (data.Get(ConstructedGateViewModel.CustomDragAndDropFormat) is IDragAbleBindable draggableBindable)
        {
            var draggable = draggableBindable.DragAble;
            draggable?.OnParentDragOver(dragEventArgs);
        }

        StageView.HideDropTarget();
    }
}