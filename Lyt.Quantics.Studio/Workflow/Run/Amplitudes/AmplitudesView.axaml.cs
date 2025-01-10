namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

public partial class AmplitudesView : UserControl
{
    public AmplitudesView()
    {
        this.InitializeComponent();
        DragDrop.SetAllowDrop(this, true);
        this.AddHandler(DragDrop.DragOverEvent, this.OnDragOver);
    }

    private void OnDragOver(object? sender, DragEventArgs dragEventArgs)
    {
        dragEventArgs.DragEffects = DragDropEffects.None;
        var data = dragEventArgs.Data;
        if (data.Get(ConstructedGateViewModel.CustomDragAndDropFormat) is IDragAbleBindable draggableBindable)
        {
            var draggable = draggableBindable.DragAble; 
            draggable?.OnParentDragOver(dragEventArgs);
        }

        StageView.HideDropTarget();
    }
}