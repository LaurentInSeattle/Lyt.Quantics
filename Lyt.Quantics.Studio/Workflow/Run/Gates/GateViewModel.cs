
namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class GateViewModel : Bindable<GateView> // : IDraggable
{
    public const string CustomDragAndDropFormat = "GateViewModel"; 

    public bool BeginDrag() 
    { 
        return true;
    }
}
