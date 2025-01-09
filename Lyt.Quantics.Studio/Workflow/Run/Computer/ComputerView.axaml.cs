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
        dragEventArgs.DragEffects = DragDropEffects.None;
        var data = dragEventArgs.Data;
        object? dragDropObject = data.Get(ConstructedGateViewModel.CustomDragAndDropFormat);
        if (dragDropObject is IDraggableBindable draggableBindable)
        {
            var draggable = draggableBindable.Draggable;
            draggable?.OnParentDragOver(dragEventArgs);

            if (this.DataContext is ComputerViewModel computerViewModel)
            {
                if (dragDropObject is IGateInfoProvider gateInfoProvider)
                {
                    if (computerViewModel.CanDrop(dragEventArgs.GetPosition(this), gateInfoProvider))
                    {
                        dragEventArgs.DragEffects = DragDropEffects.Move;
                    }
                }
            }
        }

        StageView.HideDropTarget();
    }

    private void OnDrop(object? sender, DragEventArgs dragEventArgs)
    {
        var data = dragEventArgs.Data.Get(ConstructedGateViewModel.CustomDragAndDropFormat);
        if (data is IGateInfoProvider gateInfoProvider)
        {
            if (this.DataContext is ComputerViewModel computerViewModel)
            {
                computerViewModel.OnDrop(dragEventArgs.GetPosition(this), gateInfoProvider);
            }
        }
    }
}