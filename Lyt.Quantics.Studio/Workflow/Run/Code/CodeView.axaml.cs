namespace Lyt.Quantics.Studio.Workflow.Run.Code;

public partial class CodeView : UserControl
{
    public CodeView()
    {
        this.InitializeComponent();
        this.Loaded += this.OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        DragDrop.SetAllowDrop(this, true);
        this.AddHandler(DragDrop.DragOverEvent, this.OnDragOver);
    }

    private void OnDragOver(object? sender, DragEventArgs dragEventArgs)
    {
        dragEventArgs.DragEffects = DragDropEffects.None;
        var data = dragEventArgs.Data;
        object? dragDropObject = data.Get(ConstructedGateViewModel.CustomDragAndDropFormat);
        if (dragDropObject is IDragAbleBindable draggableBindable)
        {
            var draggable = draggableBindable.DragAble;
            draggable?.OnParentDragOver(dragEventArgs);
        }

        StageView.HideDropTarget();
    }
}