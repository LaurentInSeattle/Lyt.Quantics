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
        // Debug.WriteLine("On Drag Over Gates View");
        dragEventArgs.DragEffects = DragDropEffects.None;
        var data = dragEventArgs.Data;
        if (data.Get(GateViewModel.CustomDragAndDropFormat) is GateViewModel gateViewModel)
        {
            gateViewModel.View.OnParentDragOver(dragEventArgs);
            if (this.DataContext is StageViewModel stageViewModel)
            {
                if (stageViewModel.CanDrop(dragEventArgs.GetPosition(this), gateViewModel))
                {
                    dragEventArgs.DragEffects = DragDropEffects.Move;
                }
            }
        }

        if (data.Get(GateViewModel.CustomDragAndDropFormat) is IDraggableBindable draggableBindable)
        {
            var draggable = draggableBindable.Draggable;
            draggable?.OnParentDragOver(dragEventArgs);

            if (this.DataContext is StageViewModel stageViewModel)
            {
                // if (stageViewModel.CanDrop(dragEventArgs.GetPosition(this), gateViewModel))
                {
                    dragEventArgs.DragEffects = DragDropEffects.Move;
                }
            }
        }

        // Must do this below so that the computer view is not corrupting the effect
        dragEventArgs.Handled = true;
    }

    private void OnDrop(object? sender, DragEventArgs dragEventArgs)
    {
        var data = dragEventArgs.Data.Get(GateViewModel.CustomDragAndDropFormat);
        if (data is GateViewModel gateViewModel)
        {
            if (this.DataContext is StageViewModel stageViewModel)
            {
                stageViewModel.OnDrop(dragEventArgs.GetPosition(this), gateViewModel);
            }
        }
    }
}