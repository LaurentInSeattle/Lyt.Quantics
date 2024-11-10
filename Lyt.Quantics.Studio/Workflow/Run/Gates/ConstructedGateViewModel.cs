namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class ConstructedGateViewModel : Bindable<ConstructedGateView>, IDraggableBindable
{
    public const string CustomDragAndDropFormat = "GateViewModel";
    private const string BlueBrush = "LightAqua_0_100";
    private const string OrangeBrush = "OrangePeel_2_100";
    private const double gateSize = 48.0;
    private const double spacerSize = 12.0;
    private const double largeSize = 16.0;
    private const double smallSize = 12.0;
    private const double lineSize = 2.0;

    private readonly bool isGhostViewModel;
    private readonly SolidColorBrush blueBrush;
    private readonly SolidColorBrush orangeBrush;
    private readonly SolidColorBrush backgroundBrush;
    private readonly SolidColorBrush transparentBrush;
    private readonly string gateKey;
    private readonly StageOperatorParameters stageOperatorParameters;
    private readonly List<int> allQuBitIndicesSorted;

    private static readonly Dictionary<string, string> supportedGates =
        new()
        {
            // Binary 
            { "CX", "CreateCxGate" } ,
            { "FCX", "CreateCxGate" } ,
            { "ACX", "CreateACxGate" } ,
            { "CZ","CreateCzGate" } ,
            { "Swap" , "CreateSwapGate" },

            // Ternary 
            { "CCX", "CreateCCxGate" } ,
            { "CSwap" , "CreateCSwapGate" },
            { "CCZ" , "CreateCCzGate" },

            // LATER : Controlled Gate
            // 
            //"C_" , // Controlled Gate
        };

    private readonly Grid contentGrid;

    public ConstructedGateViewModel(string gateKey, StageOperatorParameters stageOperatorParameters, bool isGhostViewModel = false)
    {
        if (!ConstructedGateViewModel.supportedGates.ContainsKey(gateKey))
        {
            throw new NotSupportedException("Not supported gate: " + gateKey);
        }

        this.gateKey = gateKey;
        this.Gate = GateFactory.Produce(gateKey, new GateParameters());
        this.stageOperatorParameters = stageOperatorParameters;
        this.isGhostViewModel = isGhostViewModel;

        this.backgroundBrush = new SolidColorBrush(color: 0x30406080);
        this.transparentBrush = new SolidColorBrush(color: 0);
        Utilities.TryFindResource(BlueBrush, out SolidColorBrush? maybeBlueBrush);
        if (maybeBlueBrush is null)
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            throw new ArgumentNullException("Could not find resource: " + BlueBrush);
#pragma warning restore CA2208 
        }
        else
        {
            this.blueBrush = maybeBlueBrush;
        }

        Utilities.TryFindResource(OrangeBrush, out SolidColorBrush? maybeOrangeBrush);
        if (maybeOrangeBrush is null)
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            throw new ArgumentNullException("Could not find resource: " + OrangeBrush);
#pragma warning restore CA2208 
        }
        else
        {
            this.orangeBrush = maybeOrangeBrush;
        }

        var allIndices = new List<int>();
        allIndices.AddRange(this.stageOperatorParameters.ControlQuBitIndices);
        allIndices.AddRange(this.stageOperatorParameters.TargetQuBitIndices);
        this.allQuBitIndicesSorted = [.. (from index in allIndices orderby index ascending select index)];

        this.contentGrid = this.CreateContentGrid();
        Rectangle rectangle = this.CreateConnectingLine();
        this.contentGrid.Children.Add(rectangle);
        this.CreateGate();
    }

    public Gate Gate { get; private set; }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        if (!this.isGhostViewModel)
        {
            this.Draggable = new Draggable();
            this.Draggable.Attach(this.View);
            this.View.Content = this.contentGrid;
            this.View.InvalidateVisual();
        }
    }

    public static bool IsGateSupported(string gateKey) 
        => ConstructedGateViewModel.supportedGates.ContainsKey(gateKey);

    private int SmallestQubitIndex => this.allQuBitIndicesSorted[0];

    private int LargestQubitIndex => this.allQuBitIndicesSorted[^1];

    #region Draggable Bindable Implementation 

    public Draggable? Draggable { get; private set; }

    public string DragDropFormat => ConstructedGateViewModel.CustomDragAndDropFormat;

    public bool OnBeginDrag()  => true;  // For now 

    public void OnEntered() 
        => this.Messenger.Publish(new GateHoverMessage(IsEnter: true, this.gateKey));    

    public void OnExited() => this.Messenger.Publish(new GateHoverMessage());

    public void OnClicked(bool isRightClick) 
    {
        if (this.Gate.IsEditable)
        {
            // Launch edit gate dialog 
            // this.Messenger.Publish(new GateEditMessage(this, isRightClick));
        }
    }
    
    public UserControl CreateGhostView()
    {
        var ghostViewModel = new ConstructedGateViewModel(
            this.gateKey, this.stageOperatorParameters, isGhostViewModel: true);
        ghostViewModel.CreateViewAndBind();
        var view = ghostViewModel.View;

        // Do NOT use 'this'.contentGrid (already has a parent and will crash Avalonia) 
        view.Content = ghostViewModel.contentGrid;
        view.ZIndex = 999_999;
        view.Opacity = 0.8;
        view.InvalidateVisual();
        return view;


        // TODO in View model 
        // Create the special graphics if needed 
        //if (GateViewModel.SpecialGateToControl(gateViewModel.Gate.CaptionKey) is Control control)
        //{
        //    this.ghostView.GateIconContent.Content = control;
        //}
    }

    #endregion Draggable Bindable Implementation 

    #region Creating Gates 

    private void CreateGate()
    {
        if (!ConstructedGateViewModel.supportedGates.TryGetValue(gateKey, out string? value))
        {
            throw new NotSupportedException("Not supported gate: " + gateKey);
        }

        string methodName = value;
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new NotSupportedException("No create method provided for gate: " + gateKey);
        }

        try
        {
            // Method will place elements in the content grid and will return nothing, but... 
            // could throw or fail.
            var methodInfo =
                this.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic)
                ?? throw new NotSupportedException("No suitable create method provided for gate: " + gateKey);
            methodInfo.Invoke(this, null);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            this.Logger.Error(ex.ToString());
            throw;
        }
    }

#pragma warning disable IDE0051 
    // Remove unused private members
    // USED ==> Invoked using reflection: see CreateGate() above

    private void CreateCxGate()
    {
        var parameters = this.stageOperatorParameters;
        var control = this.CreateControlDot();
        int controlIndex = parameters.ControlQuBitIndices[0];
        this.PlaceGridAt(control, controlIndex);
        var not = this.CreateCNot();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(not, targetIndex);
    }

    private void CreateACxGate()
    {
        var parameters = this.stageOperatorParameters;
        var control = this.CreateAntiControlDot();
        int controlIndex = parameters.ControlQuBitIndices[0];
        this.PlaceGridAt(control, controlIndex);
        var not = this.CreateCNot();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(not, targetIndex);
    }

    private void CreateSwapGate()
    {
        var parameters = this.stageOperatorParameters;
        var first = this.CreateHalfSwap();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(first, targetIndex);
        var last = this.CreateHalfSwap();
        targetIndex = parameters.TargetQuBitIndices[1];
        this.PlaceGridAt(last, targetIndex);
    }

    private void CreateCzGate()
    {
        var parameters = this.stageOperatorParameters;
        var first = this.CreateControlDot();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(first, targetIndex);
        var last = this.CreateControlDot();
        targetIndex = parameters.TargetQuBitIndices[1];
        this.PlaceGridAt(last, targetIndex);
    }

    private void CreateCCzGate()
    {
        var parameters = this.stageOperatorParameters;
        var first = this.CreateControlDot();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(first, targetIndex);
        var second = this.CreateControlDot();
        targetIndex = parameters.TargetQuBitIndices[1];
        this.PlaceGridAt(second, targetIndex);
        var last = this.CreateControlDot();
        targetIndex = parameters.TargetQuBitIndices[2];
        this.PlaceGridAt(last, targetIndex);
    }

    private void CreateCCxGate()
    {
        var parameters = this.stageOperatorParameters;
        var first = this.CreateControlDot();
        int targetIndex = parameters.ControlQuBitIndices[0];
        this.PlaceGridAt(first, targetIndex);
        var second = this.CreateControlDot();
        targetIndex = parameters.ControlQuBitIndices[1];
        this.PlaceGridAt(second, targetIndex);
        var last = this.CreateCNot();
        targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(last, targetIndex);
    }

    private void CreateCSwapGate()
    {
        var parameters = this.stageOperatorParameters;
        var first = this.CreateControlDot();
        int firstIndex = parameters.ControlQuBitIndices[0];
        this.PlaceGridAt(first, firstIndex);
        var second = this.CreateHalfSwap();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(second, targetIndex);
        var last = this.CreateHalfSwap();
        targetIndex = parameters.TargetQuBitIndices[1];
        this.PlaceGridAt(last, targetIndex);
    }

#pragma warning restore IDE0051 

    #endregion Creating Gates 

    #region Creating Gate Elements 

    private Grid CreateContentGrid()
    {
        int spacerCount = this.LargestQubitIndex - this.SmallestQubitIndex;
        int gateCount = 1 + spacerCount;
        double height = gateCount * gateSize + spacerCount * spacerSize;
        this.GateHeight = height;
        var grid = new Grid()
        {
            Height = height,
            Width = gateSize,
#if DEBUG
            // ShowGridLines = true,
#endif
        };

        var border = new Border()
        {
            Margin = new Thickness(2.0),
            CornerRadius = new CornerRadius(4.0),
            Background = this.backgroundBrush,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        var rowDefinitions = new RowDefinitions
        {
            new RowDefinition(gateSize, GridUnitType.Pixel)
        };

        for (int i = 0; i < spacerCount; i++)
        {
            rowDefinitions.Add(new RowDefinition(spacerSize, GridUnitType.Pixel));
            rowDefinitions.Add(new RowDefinition(gateSize, GridUnitType.Pixel));
        }

        grid.RowDefinitions = rowDefinitions;
        border.SetValue(Grid.RowSpanProperty, rowDefinitions.Count);
        grid.Children.Add(border);

        return grid;
    }

    private void PlaceGridAt(Grid grid, int qubitIndex)
    {
        int baseIndex = qubitIndex - this.SmallestQubitIndex;
        int gridRow = baseIndex * 2;
        grid.SetValue(Grid.RowProperty, gridRow);
        this.contentGrid.Children.Add(grid);
    }

    private Rectangle CreateConnectingLine()
    {
        int spacerCount = this.LargestQubitIndex - this.SmallestQubitIndex;
        int gateCount = 1 + spacerCount;
        double height = gateCount * gateSize + spacerCount * spacerSize - gateSize;
        var rectangle = new Rectangle()
        {
            Height = height,
            Width = lineSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = this.blueBrush,
        };

        int gridRows = gateCount + spacerCount;
        rectangle.SetValue(Grid.RowSpanProperty, gridRows);
        return rectangle;
    }

    private Grid CreateControlDot()
    {
        var ellipse = new Ellipse()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Height = smallSize,
            Width = smallSize,
            Fill = this.blueBrush,
        };

        var grid = new Grid()
        {
            Height = gateSize,
            Width = gateSize,
        };

        grid.Children.Add(ellipse);
        return grid;
    }

    private Grid CreateAntiControlDot()
    {
        var ellipse = new Ellipse()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Height = smallSize,
            Width = smallSize,
            Fill = this.orangeBrush,
            Stroke = this.blueBrush,
            StrokeThickness = 2.0,
        };

        var grid = new Grid()
        {
            Height = gateSize,
            Width = gateSize,
        };

        grid.Children.Add(ellipse);
        return grid;
    }

    private Grid CreateHalfSwap()
    {
        var rectangle1 = new Rectangle()
        {
            Height = largeSize,
            Width = lineSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = this.blueBrush,
            Stroke = this.blueBrush,
            RenderTransform = new RotateTransform(angle: 45.0),
        };

        var rectangle2 = new Rectangle()
        {
            Height = lineSize,
            Width = largeSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = this.blueBrush,
            Stroke = this.blueBrush,
            RenderTransform = new RotateTransform(angle: 45.0),
        };

        var grid = new Grid()
        {
            Height = gateSize,
            Width = gateSize,
        };

        grid.Children.Add(rectangle1);
        grid.Children.Add(rectangle2);
        return grid;
    }

    private Grid CreateCNot()
    {
        var ellipse = new Ellipse()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Height = largeSize,
            Width = largeSize,
            Stroke = this.blueBrush,
            StrokeThickness = lineSize,
            Fill = this.transparentBrush,
        };

        var rectangle1 = new Rectangle()
        {
            Height = largeSize,
            Width = lineSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = this.blueBrush,
        };

        var rectangle2 = new Rectangle()
        {
            Height = lineSize,
            Width = largeSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = this.blueBrush,
        };

        var grid = new Grid()
        {
            Height = gateSize,
            Width = gateSize,
        };

        grid.Children.Add(rectangle1);
        grid.Children.Add(rectangle2);
        grid.Children.Add(ellipse);
        return grid;
    }

    #endregion Gate Elements 

    #region Bound Properties 

    public double GateHeight { get => this.Get<double>(); set => this.Set(value); }

    #endregion Bound Properties 
}
