namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public partial class GateView : UserControl
{
    private readonly bool isGhost;

    private bool isPointerPressed;
    private bool isDragging;
    private PointerPoint pointerPressedPoint;
    private UserControl? ghostView;

    public GateView() : this(isGhost: false) { }

    public GateView(bool isGhost)
    {
        this.isGhost = isGhost;
        this.InitializeComponent();
        if (!isGhost)
        {
            // Don't do that for the ghost view 
            this.DataContextChanged += this.OnGateViewDataContextChanged;
            this.HookPointerEvents();
        }
    }

    private void OnGateViewDataContextChanged(object? sender, EventArgs e)
    {
        if (this.DataContext is GateViewModel gateViewModel)
        {
            gateViewModel.BindOnDataContextChanged(this);
        }
    }

    private void HookPointerEvents()
    {
        this.PointerPressed += this.OnPointerPressed;
        this.PointerReleased += this.OnPointerReleased;
        this.PointerMoved += this.OnPointerMoved;
    }

    private void UnhookPointerEvents()
    {
        this.PointerPressed -= this.OnPointerPressed;
        this.PointerReleased -= this.OnPointerReleased;
        this.PointerMoved -= this.OnPointerMoved;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs pointerPressedEventArgs)
    {
        Debug.WriteLine("Pressed");
        this.isPointerPressed = true;
        this.pointerPressedPoint = pointerPressedEventArgs.GetCurrentPoint(this);
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
            Point currentPosition = pointerEventArgs.GetPosition(this);
            var distance = Point.Distance(currentPosition, pointerPressedPoint.Position);
            if (distance <= 4.2)
            {
                Debug.WriteLine("Too close.");
                return;
            }

            this.BeginDrag(pointerEventArgs);
        }
    }

    private void OnPointerReleased(object? sender, PointerReleasedEventArgs _)
    {
        Debug.WriteLine("Released");
        this.isPointerPressed = false;
    }

    private void BeginDrag(PointerEventArgs pointerEventArgs)
    {
        Debug.WriteLine("Try Begin Drag");

        if (this.isDragging)
        {
            return;
        }

        //if ((this.DataContext is not null) && ( this.DataContext is IDraggable) )
        if ((this.DataContext is not null) && (this.DataContext is GateViewModel gateViewModel))
        {
            bool allowDrag = gateViewModel.BeginDrag();
            if (!allowDrag)
            {
                Debug.WriteLine("Dragging rejected");
                return;
            }
        }
        else
        {
            Debug.WriteLine("No data context");
            return;
        }


        Debug.WriteLine("Drag == true ");
        this.isDragging = true;

        this.ghostView = new GateView(isGhost: true)
        {
            DataContext = gateViewModel,
            Opacity = 0.8,
            ZIndex = 999_999,
        };

        if (!this.ValidateGhost(out Canvas? canvas))
        {
            Debug.WriteLine("No canvas");
            return;
        }

        // Launch the DragDrop task, fire and forget 
        this.DoDragDrop(pointerEventArgs, gateViewModel, canvas!);
    }

    private async void DoDragDrop(PointerEventArgs pointerEventArgs, GateViewModel gateViewModel, Canvas canvas)
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
        dragData.Set(GateViewModel.CustomDragAndDropFormat, gateViewModel);
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
        if (this.isGhost)
        {
            return false;
        }

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
}