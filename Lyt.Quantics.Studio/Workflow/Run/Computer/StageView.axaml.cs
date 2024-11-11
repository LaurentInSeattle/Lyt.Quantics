namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public partial class StageView : UserControl
{
    public StageView()
    {
        this.InitializeComponent();
        this.DataContextChanged += this.OnStageViewDataContextChanged;
        this.AddHandler(DragDrop.DragOverEvent, this.OnDragOver);
        this.AddHandler(DragDrop.DropEvent, this.OnDrop);
    }

    private void OnStageViewDataContextChanged(object? sender, EventArgs e)
    {
        if (this.DataContext is StageViewModel stageViewModel)
        {
            stageViewModel.BindOnDataContextChanged(this);
        }
    }

    private void OnDragOver(object? sender, DragEventArgs dragEventArgs)
    {
        // Debug.WriteLine("Dragging over stage view");

        dragEventArgs.DragEffects = DragDropEffects.None;
        var data = dragEventArgs.Data;
        object? dragDropObject = data.Get(ConstructedGateViewModel.CustomDragAndDropFormat); 
        if (dragDropObject is IDraggableBindable draggableBindable)
        {
            // Debug.WriteLine("Drag object is IDraggableBindable");
            var draggable = draggableBindable.Draggable;
            draggable?.OnParentDragOver(dragEventArgs);
            if (this.DataContext is StageViewModel stageViewModel)
            {
                if (dragDropObject is IGateInfoProvider gateInfoProvider)
                {
                    // Debug.WriteLine("Drag object is IGateInfoProvider");
                    if (stageViewModel.CanDrop(dragEventArgs.GetPosition(this), gateInfoProvider))
                    {
                        // Debug.WriteLine("Drag object can be dropped ");
                        dragEventArgs.DragEffects = DragDropEffects.Move;
                    }
                    else
                    {
                        // Debug.WriteLine("Drop rejected ");
                    }
                }
            }

            // Must do this below so that the computer view is not corrupting the effect
            dragEventArgs.Handled = true;
            // Debug.WriteLine("Event handled: Dragging over stage view");
        }

    }

    private void OnDrop(object? sender, DragEventArgs dragEventArgs)
    {
        var data = dragEventArgs.Data.Get(ConstructedGateViewModel.CustomDragAndDropFormat);
        if (data is IGateInfoProvider gateInfoProvider)
        {
            if (this.DataContext is StageViewModel stageViewModel)
            {
                stageViewModel.OnDrop(dragEventArgs.GetPosition(this), gateInfoProvider);
                dragEventArgs.Handled = true;
            }
        }
    }
}