namespace Lyt.Quantics.Studio.Behaviors.DragDrop;

using global::Avalonia.Input;

/// <summary> 
/// Behaviour for controls and views that should support visualising the 'ghost' view of 
/// 'DragAble' objects that are dragged around. 
/// </summary>
public class DragOverAble(
    Action? hideDropTarget = null,
    Func<IDropTarget, Point, bool>? showDropTarget = null)
        : BehaviorBase<BehaviorEnabledUserControl>
{
    private readonly Action? hideDropTarget = hideDropTarget;
    private readonly Func<IDropTarget, Point, bool>? showDropTarget = showDropTarget;

    protected override void OnAttached()
    {
        BehaviorEnabledUserControl userControl = base.GuardAssociatedObject();
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

    private void OnDragOver(object? sender, DragEventArgs dragEventArgs)
    {
        dragEventArgs.DragEffects = DragDropEffects.None;
        if (this.AssociatedObject is not UserControl userControl)
        {
            return;
        }

        bool showedDropTarget = false;
        var data = dragEventArgs.Data;
        var formats = data.GetDataFormats().ToList();
        if (formats is not null && formats.Count > 0)
        {
            foreach (var format in formats)
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
                        Point position = dragEventArgs.GetPosition(userControl);
                        if (dropTarget.CanDrop(position, dragDropObject))
                        {
                            dragEventArgs.DragEffects = DragDropEffects.Move;
                            if (this.showDropTarget is not null)
                            {
                                showedDropTarget = this.showDropTarget.Invoke(dropTarget, position);
                            }
                        }
                    }

                    break;
                }
            }
        }

        if (!showedDropTarget)
        {
            this.hideDropTarget?.Invoke();
        }

        dragEventArgs.Handled = true;
    }
}
