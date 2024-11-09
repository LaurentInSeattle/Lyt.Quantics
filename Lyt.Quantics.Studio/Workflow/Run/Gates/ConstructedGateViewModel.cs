namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class ConstructedGateViewModel : Bindable<ConstructedGateView>
{
    private const string BlueBrush = "LightAqua_0_100";
    private const string OrangeBrush = "OrangePeel_2_100";
    private const double gateSize = 48.0;
    private const double spacerSize = 12.0;
    private const double largeSize = 16.0;
    private const double smallSize = 12.0;
    private const double lineSize = 2.0;

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
            //"CCX",
            //"FCX",
            //"CSwap",
            { "CCZ" , "CreateCCzGate" },

            // LATER 
            // 
            //"C_" , // Controlled Gate
        };

    private readonly Grid contentGrid;

    public ConstructedGateViewModel(string gateKey, StageOperatorParameters stageOperatorParameters)
    {
        if (!ConstructedGateViewModel.supportedGates.ContainsKey(gateKey))
        {
            throw new NotSupportedException("Not supported gate: " + gateKey);
        }

        this.gateKey = gateKey;
        this.stageOperatorParameters = stageOperatorParameters;

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

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.View.Content = this.contentGrid;
        this.View.InvalidateVisual();
    }

    public static bool IsGateSupported(string gateKey) 
        => ConstructedGateViewModel.supportedGates.ContainsKey(gateKey);

    private int SmallestQubitIndex => this.allQuBitIndicesSorted[0];

    private int LargestQubitIndex => this.allQuBitIndicesSorted[^1];

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

    #region Creating Gates 

#pragma warning disable IDE0051 
    // Remove unused private members
    // USED ==> Invoked using reflection: see CreateGate()

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
            Background = this.backgroundBrush,
#if DEBUG
            // ShowGridLines = true,
#endif
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
