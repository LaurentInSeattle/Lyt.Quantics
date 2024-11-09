namespace Lyt.Quantics.Studio.Behaviors.DragDrop;

using global::Avalonia.Input;

public sealed class Draggable : BehaviorBase<BehaviorEnabledUserControl>
{
    private bool isPointerPressed;
    private bool isDragging;
    private PointerPoint pointerPressedPoint;
    private UserControl? ghostView;

    private IDraggableBindable? draggableBindable;

    protected override void OnAttached()
    {
        _ = this.GuardAssociatedObject();
        Debug.WriteLine("On attached to: " + this.DraggableBindable.GetType().Name);
        this.HookPointerEvents();
    }

    protected override void OnDetaching() => this.UnhookPointerEvents();

    public BehaviorEnabledUserControl UserControl => this.GuardAssociatedObject();

    public IDraggableBindable DraggableBindable
    {
        get => this.draggableBindable is not null ?
            this.draggableBindable :
            throw new InvalidOperationException("Not attached or invalid asociated object.");
        private set => this.draggableBindable = value;
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
            (userControl.DataContext is null) ||
            (userControl.DataContext is not IDraggableBindable iDraggableBindable))
        {
            throw new InvalidOperationException("Not attached or invalid asociated object.");
        }
#pragma warning restore IDE0019 

        DraggableBindable = iDraggableBindable;
        return userControl;
    }

    private void HookPointerEvents()
    {
        BehaviorEnabledUserControl userControl = this.UserControl;
        userControl.PointerPressed += this.OnPointerPressed;
        userControl.PointerReleased += this.OnPointerReleased;
        userControl.PointerMoved += this.OnPointerMoved;
        userControl.PointerEntered += this.OnPointerEntered;
        userControl.PointerExited += this.OnPointerExited;
    }

    private void UnhookPointerEvents()
    {
        BehaviorEnabledUserControl userControl = this.UserControl;
        userControl.PointerPressed -= this.OnPointerPressed;
        userControl.PointerReleased -= this.OnPointerReleased;
        userControl.PointerMoved -= this.OnPointerMoved;
        userControl.PointerEntered -= this.OnPointerEntered;
        userControl.PointerExited -= this.OnPointerExited;
    }

    private void OnPointerEntered(object? sender, PointerEventArgs pointerEventArgs)
        => this.DraggableBindable.OnEntered();

    private void OnPointerExited(object? sender, PointerEventArgs pointerEventArgs)
        => this.DraggableBindable.OnExited();

    private void OnPointerPressed(object? sender, PointerPressedEventArgs pointerPressedEventArgs)
    {
        Debug.WriteLine("Pressed");
        BehaviorEnabledUserControl userControl = this.UserControl;
        this.isPointerPressed = true;
        this.pointerPressedPoint = pointerPressedEventArgs.GetCurrentPoint(userControl);
    }

    private void OnPointerMoved(object? sender, PointerEventArgs pointerEventArgs)
    {
        if (!this.isPointerPressed)
        {
            this.isPointerPressed = false;
            return;
        }

        if (this.isDragging)
        {
            Debug.WriteLine("Dragging...");
            this.AdjustGhostPosition(pointerEventArgs);
            return;
        }
        else
        {
            Debug.WriteLine("Moving...");
            BehaviorEnabledUserControl userControl = this.UserControl;
            Point currentPosition = pointerEventArgs.GetPosition(userControl);
            var distance = Point.Distance(currentPosition, pointerPressedPoint.Position);
            if (distance <= 4.2)
            {
                Debug.WriteLine("Too close.");
                return;
            }

            this.BeginDrag(pointerEventArgs);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs args)
    {
        Debug.WriteLine("Released");
        if (this.isDragging || !this.isPointerPressed)
        {
            return;
        }

        this.isPointerPressed = false;

        // It's a Click 
        // The view model will decide whether or not the object is editable
        // For gates: Check if toolbox, parameters, etc...
        bool isRightClick = args.InitialPressMouseButton == MouseButton.Right;
        this.DraggableBindable.OnClicked(isRightClick);
    }

    private void BeginDrag(PointerEventArgs pointerEventArgs)
    {
        Debug.WriteLine("Try Begin Drag");

        if (this.isDragging)
        {
            return;
        }

        _ = this.GuardAssociatedObject();
        bool allowDrag = this.DraggableBindable.OnBeginDrag();
        if (!allowDrag)
        {
            Debug.WriteLine("Dragging rejected");
            return;
        }

        Debug.WriteLine("Drag == true ");
        this.isDragging = true;

        // Create the ghost view  
        this.ghostView = this.DraggableBindable.CreateGhostView();

        // TODO in View model 
        // Create the special graphics if needed 
        //if (GateViewModel.SpecialGateToControl(gateViewModel.Gate.CaptionKey) is Control control)
        //{
        //    this.ghostView.GateIconContent.Content = control;
        //}

        if (!this.ValidateGhost(out Canvas? canvas))
        {
            Debug.WriteLine("No canvas");
            return;
        }

        // Launch the DragDrop task, fire and forget 
        this.DoDragDrop(pointerEventArgs, canvas!);
    }

    private async void DoDragDrop(PointerEventArgs pointerEventArgs, Canvas canvas)
    {
        if (this.ghostView is null)
        {
            Debug.WriteLine("No ghost");
            return;
        }

        this.UnhookPointerEvents();
        canvas.Children.Add(this.ghostView);
        this.AdjustGhostPosition(pointerEventArgs.GetPosition(canvas));

        var dragData = new DataObject();
        string dragAndDropFormat = this.DraggableBindable.DragDropFormat;
        dragData.Set(dragAndDropFormat, this.DraggableBindable);

        var result = await DragDrop.DoDragDrop(pointerEventArgs, dragData, DragDropEffects.Move);
        Debug.WriteLine($"DragAndDrop result: {result}");

        canvas.Children.Remove(this.ghostView);
        this.ghostView.DataContext = null;
        this.ghostView = null;

        Debug.WriteLine("Drag == false");
        this.isPointerPressed = false;
        this.isDragging = false;
        this.HookPointerEvents();
    }

    private void AdjustGhostPosition(Point position)
    {
        // Debug.WriteLine("AdjustGhostPosition from point");
        if (!this.ValidateGhost(out Canvas? _))
        {
            return;
        }

        Point newPosition = new(position.X + 4, position.Y + 4);
        this.ghostView!.SetValue(Canvas.LeftProperty, newPosition.X);
        this.ghostView.SetValue(Canvas.TopProperty, newPosition.Y);
    }

    private void AdjustGhostPosition(PointerEventArgs pointerEventArgs)
    {
        // Debug.WriteLine("AdjustGhostPosition from pointer");
        if (!this.ValidateGhost(out Canvas? canvas))
        {
            return;
        }

        Point position = pointerEventArgs.GetPosition(canvas!);
        Point newPosition = new(position.X + 4, position.Y + 4);
        this.ghostView!.SetValue(Canvas.LeftProperty, newPosition.X);
        this.ghostView.SetValue(Canvas.TopProperty, newPosition.Y);
    }

    private bool ValidateGhost(out Canvas? canvas)
    {
        canvas = null;
        if (this.ghostView is null)
        {
            Debug.WriteLine("No ghost");
            return false;
        }

        if (App.MainWindow is not MainWindow mainWindow)
        {
            Debug.WriteLine("No main window");
            return false;
        }

        canvas = mainWindow.MainWindowCanvas;
        if (canvas is null)
        {
            Debug.WriteLine("No grid in main window");
            return false;
        }

        return true;
    }

    public void OnParentDragOver(DragEventArgs dragEventArgs)
    {
        // Debug.WriteLine("On Drag Over Gate");
        if (!this.ValidateGhost(out Canvas? canvas))
        {
            return;
        }

        Point position = dragEventArgs.GetPosition(canvas!);
        this.AdjustGhostPosition(position);
    }
}
