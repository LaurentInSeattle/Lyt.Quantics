namespace Lyt.Quantics.Studio.Behaviors.DragDrop;

/// <summary> Interface contract for bindables (VM) that have a view that can be dragged </summary>
public interface IDragAbleBindable
{
    void OnEntered();

    void OnExited();

    void OnLongPress();

    void OnClicked(bool isRightClick);

    bool OnBeginDrag();

    UserControl CreateGhostView();

    string DragDropFormat { get; }

    DragAble? DragAble { get; }
}
