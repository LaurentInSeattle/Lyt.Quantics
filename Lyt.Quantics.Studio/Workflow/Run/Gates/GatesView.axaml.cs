
namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public partial class GatesView : UserControl
{
    public GatesView()
    {
        this.InitializeComponent();
        this.Loaded += this.OnLoaded;
    }

    private void OnLoaded(object? sender, RoutedEventArgs e)
    {
        DragDrop.SetAllowDrop(this, true);
        this.AddHandler(DragDrop.DragOverEvent, this.OnDragOver);
        this.AddHandler(DragDrop.DropEvent, this.OnDrop);
    }

    private void OnDrop(object? sender, DragEventArgs dragEventArgs)
    {
        var data = dragEventArgs.Data.Get(GateViewModel.CustomDragAndDropFormat);
        if ((data is GateViewModel gateViewModel) && (!gateViewModel.IsToolbox)) 
        {
            if (this.DataContext is GatesViewModel gatesViewModel)
            {
                gatesViewModel.OnDrop(dragEventArgs.GetPosition(this), gateViewModel);
            }
        }
    }

    private void OnDragOver(object? sender, DragEventArgs dragEventArgs)
    {
        var data = dragEventArgs.Data;
        if (data.Get(GateViewModel.CustomDragAndDropFormat) is GateViewModel gateViewModel)
        {
            gateViewModel.View.OnParentDragOver(dragEventArgs);
            dragEventArgs.DragEffects = 
                gateViewModel.IsToolbox ? DragDropEffects.None: DragDropEffects.Move;
        }
        else
        {
            dragEventArgs.DragEffects = DragDropEffects.None;
        } 
    }
}