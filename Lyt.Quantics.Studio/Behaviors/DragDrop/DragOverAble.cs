﻿namespace Lyt.Quantics.Studio.Behaviors.DragDrop;

using global::Avalonia.Input;

/// <summary> 
/// Behaviour for controls and views that should support visualising the 'ghost' view of 
/// 'DragAble' objects that are dragged around. 
/// </summary>
public class DragOverAble(Action hideDropTarget) : BehaviorBase<BehaviorEnabledUserControl>
{
    private readonly Action hideDropTarget = hideDropTarget;

    protected override void OnAttached()
    {
        BehaviorEnabledUserControl userControl = this.GuardAssociatedObject();
        DragDrop.SetAllowDrop(userControl, true);
        userControl.AddHandler(DragDrop.DragOverEvent, this.OnDragOver);
    }

    protected override void OnDetaching()
    {
        if (this.AssociatedObject is UserControl userControl)
        {
            DragDrop.SetAllowDrop(userControl, false);
            userControl.RemoveHandler(DragDrop.DragOverEvent, this.OnDragOver);
        } 
    }

    private BehaviorEnabledUserControl GuardAssociatedObject()
    {
        if (this.AssociatedObject is null)
        {
            throw new InvalidOperationException("Not attached.");
        }

        if (!this.AssociatedObject.GetType().DerivesFrom<BehaviorEnabledUserControl>())
        {
            throw new InvalidOperationException("Invalid asociated object.");
        }

#pragma warning disable IDE0019 // Use pattern matching
        // VS BUG => Turns out that pattern matching cannot be used here !
        var userControl = this.AssociatedObject as BehaviorEnabledUserControl;
        if ((userControl is null) ||
            (userControl.DataContext is null))
        {
            throw new InvalidOperationException("Not attached or invalid asociated object.");
        }
#pragma warning restore IDE0019 

        return userControl;
    }

    private void OnDragOver(object? sender, DragEventArgs dragEventArgs)
    {
        dragEventArgs.DragEffects = DragDropEffects.None;
        if (this.AssociatedObject is not UserControl userControl)
        {
            return; 
        }

        var data = dragEventArgs.Data;
        var formats = data.GetDataFormats().ToList();
        if ( formats is not null && formats.Count > 0)
        {
            foreach (var format  in formats)
            {
                object? dragDropObject = data.Get(format);
                if (dragDropObject is IDragAbleBindable draggableBindable)
                {
                    var draggable = draggableBindable.DragAble;
                    if (draggable is null)
                    {
                        break; 
                    } 

                    draggable.OnParentDragOver(dragEventArgs);

                    if (userControl.DataContext is IDropTarget dropTarget)
                    {
                        if (dropTarget.CanDrop(dragEventArgs.GetPosition(userControl), dragDropObject))
                        {
                            dragEventArgs.DragEffects = DragDropEffects.Move;
                        }
                    }

                    break;
                }
            }
        }

        this.hideDropTarget?.Invoke();
    }
}
