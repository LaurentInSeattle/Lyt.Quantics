namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

public partial class AmplitudesView : UserControl
{
    public AmplitudesView()
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
        if (data.Get(GateViewModel.CustomDragAndDropFormat) is GateViewModel gateViewModel)
        {
            gateViewModel.View.OnParentDragOver(dragEventArgs);
        }
    }
}