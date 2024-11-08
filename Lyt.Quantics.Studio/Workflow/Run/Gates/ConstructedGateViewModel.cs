namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class ConstructedGateViewModel : Bindable<ConstructedGateView>
{
    private const string BlueBrush = "LightAqua_1_100";
    private const double gateSize = 48.0;
    private const double spacerSize = 12.0;
    private const double largeSize = 16.0;
    private const double smallSize = 12.0;
    private const double lineSize = 2.0;

    private readonly SolidColorBrush blueBrush;
    private readonly SolidColorBrush debugBrush;
    private readonly SolidColorBrush transparentBrush;
    private readonly string gateKey;
    private readonly StageOperatorParameters stageOperatorParameters;
    private readonly List<int> allQuBitIndicesSorted;

    private readonly Dictionary<string, string> supportedGates =
        new()
        {
            // Binary 
            { "CX", "CreateCxGate" } ,
            //"CZ",
            //"FCX",
            //"Swap",

            //// Ternary 
            //"CCX",
            //"CCZ",
            //"FCX",
            //"CSwap",

            // LATER 
            // 
            //"C_" , // Controlled Gate
            //"ACX",
        };

    private Grid contentGrid;

    public ConstructedGateViewModel(string gateKey, StageOperatorParameters stageOperatorParameters)
    {
        if (!this.supportedGates.ContainsKey(gateKey))
        {
            throw new NotSupportedException("Not supported gate: " + gateKey);
        }

        this.gateKey = gateKey;
        this.stageOperatorParameters = stageOperatorParameters;

        this.debugBrush = new SolidColorBrush(color: 0x40406080);
        this.transparentBrush = new SolidColorBrush(color: 0);
        Utilities.TryFindResource(BlueBrush, out SolidColorBrush? brush);
        if (brush is null)
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            throw new ArgumentNullException("Could not find resource: " + BlueBrush);
#pragma warning restore CA2208 
        }
        else
        {
            this.blueBrush = brush;
        }

        var allIndices = new List<int>();
        allIndices.AddRange(this.stageOperatorParameters.ControlQuBitIndices);
        allIndices.AddRange(this.stageOperatorParameters.TargetQuBitIndices);
        this.allQuBitIndicesSorted = [.. (from index in allIndices orderby index ascending select index)];
        this.contentGrid = new Grid();
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.contentGrid = this.CreateContentGrid();
        Rectangle rectangle = this.CreateConnectingLine();
        this.contentGrid.Children.Add(rectangle);
        Grid grid = this.CreateGate(); 
        this.contentGrid.Children.Add(grid);
        this.View.Content = this.contentGrid;
    }

    private int SmallestQubitIndex => this.allQuBitIndicesSorted[0];

    private int LargestQubitIndex => this.allQuBitIndicesSorted[^1];

    private Grid CreateGate()
    {
        if (!this.supportedGates.ContainsKey(gateKey))
        {
            throw new NotSupportedException("Not supported gate: " + gateKey);
        }

        string methodName = this.supportedGates[gateKey];
        if (string.IsNullOrWhiteSpace(methodName))
        {
            throw new NotSupportedException("No create method provided for gate: " + gateKey);
        }

        try
        {
            var methodInfo = 
                this.GetType().GetMethod(
                    methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            if (methodInfo == null)
            {
                throw new NotSupportedException("No suitable create method provided for gate: " + gateKey);
            }

            object? created = methodInfo.Invoke(this, null); 
            if ( created is Grid grid)
            {
                return grid; 
            }

            throw new Exception("Failed to create gate: " + gateKey);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            this.Logger.Error(ex.ToString()); 
            throw;
        } 
    }

    private void CreateCxGate()
    {
    }

    #region Gate Elements 

    private void PlaceGridAt(Grid grid, int qubitIndex)
    {
        int baseIndex = qubitIndex - this.SmallestQubitIndex;
        int gridRow = baseIndex * 2;
        grid.SetValue(Grid.RowProperty, gridRow);
        this.contentGrid.Children.Add(grid);
    }

    private Grid CreateContentGrid()
    {
        int spacerCount = this.LargestQubitIndex - this.SmallestQubitIndex;
        int gateCount = 1 + spacerCount;
        double height = gateCount * gateSize + spacerCount * spacerSize;
        var grid = new Grid()
        {
            Height = height,
            Width = gateSize,
#if DEBUG
            Background = this.debugBrush,
            ShowGridLines = true,
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

        double gridRows = gateCount + spacerCount;
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

    private Grid CreateHalfSwap()
    {
        var rectangle1 = new Rectangle()
        {
            Height = largeSize,
            Width = lineSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = this.blueBrush,
            RenderTransform = new RotateTransform(angle: 45.0),
        };

        var rectangle2 = new Rectangle()
        {
            Height = lineSize,
            Width = largeSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = this.blueBrush,
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

/*
 ANTI-Control Ellipse : Most likely not needed

		<Ellipse
			Grid.Row="0" Grid.RowSpan="2"
			VerticalAlignment="Center" HorizontalAlignment="Center"
			Height="12" Width="12"
			Fill="AntiqueWhite"		
			Stroke="{StaticResource ResourceKey= LightAqua_2_100}"		
			StrokeThickness="2"
			/>
 */