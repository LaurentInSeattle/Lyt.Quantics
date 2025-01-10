namespace Lyt.Quantics.Studio.Behaviors.DragDrop;

/// <summary> Interface contract for bindables (VM) that support the drop of an object.</summary>
public interface IDropTarget
{
    bool CanDrop(Point point, object droppedObject);

    void OnDrop(Point point, object droppedObject); 
}
