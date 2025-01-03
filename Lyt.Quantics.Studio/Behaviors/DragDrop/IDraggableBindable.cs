﻿namespace Lyt.Quantics.Studio.Behaviors.DragDrop; 

public interface IDraggableBindable
{
    void OnEntered();

    void OnExited();

    void OnLongPress();

    void OnClicked(bool isRightClick);

    bool OnBeginDrag();

    UserControl CreateGhostView();

    string DragDropFormat { get; }

    Draggable? Draggable { get; }
}
