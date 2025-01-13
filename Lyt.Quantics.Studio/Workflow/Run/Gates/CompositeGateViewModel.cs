﻿namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

/// <summary> Abstract base class for Controlled and Constructed gates  </summary>
/// <typeparam name="TView">The corresponding view.</typeparam>
public abstract class CompositeGateViewModel<TView> : GateViewModelBase<TView>
        where TView : Control, new()
{
    protected const string BlueBrush = "LightAqua_0_100";
    protected const string OrangeBrush = "OrangePeel_2_100";

    protected const double gateSize = 48.0;
    protected const double spacerSize = 12.0;
    protected const double largeSize = 16.0;
    protected const double smallSize = 12.0;
    protected const double lineSize = 2.0;

    protected static readonly SolidColorBrush blueBrush;
    protected static readonly SolidColorBrush orangeBrush;
    protected static readonly SolidColorBrush backgroundBrush;
    protected static readonly SolidColorBrush transparentBrush;

    protected readonly Grid contentGrid;
    protected readonly List<int> allQuBitIndicesSorted;

    static CompositeGateViewModel()
    {
        backgroundBrush = new SolidColorBrush(color: 0x30406080);
        transparentBrush = new SolidColorBrush(color: 0);
        Utilities.TryFindResource(BlueBrush, out SolidColorBrush? maybeBlueBrush);
        if (maybeBlueBrush is null)
        {
#pragma warning disable CA2208 // Instantiate argument exceptions correctly
            throw new ArgumentNullException("Could not find resource: " + BlueBrush);
#pragma warning restore CA2208 
        }
        else
        {
            blueBrush = maybeBlueBrush;
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
            orangeBrush = maybeOrangeBrush;
        }
    }

    public CompositeGateViewModel(
        Gate gate, int stageIndex, QubitsIndices qubitsIndices, bool isGhost = false)
        : base(gate, qubitsIndices, isGhost, isToolBox: false, stageIndex)
        
    {
        this.allQuBitIndicesSorted = [.. this.QubitsIndices.AllQubitIndicesSorted()];
        this.contentGrid = this.CreateContentGrid();
    }

    protected int SmallestQubitIndex => this.allQuBitIndicesSorted[0];

    protected int LargestQubitIndex => this.allQuBitIndicesSorted[^1];

    #region Creating Gate Elements 

    protected Grid CreateContentGrid()
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
            Background = backgroundBrush,
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

    protected void PlaceGridAt(Grid grid, int qubitIndex)
    {
        int baseIndex = qubitIndex - this.SmallestQubitIndex;
        int gridRow = baseIndex * 2;
        grid.SetValue(Grid.RowProperty, gridRow);
        this.contentGrid.Children.Add(grid);
    }

    protected Rectangle CreateConnectingLine()
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
            Fill = blueBrush,
        };

        int gridRows = gateCount + spacerCount;
        rectangle.SetValue(Grid.RowSpanProperty, gridRows);
        return rectangle;
    }

    protected Grid CreateControlDot()
    {
        var ellipse = new Ellipse()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Height = smallSize,
            Width = smallSize,
            Fill = blueBrush,
        };

        var grid = new Grid()
        {
            Height = gateSize,
            Width = gateSize,
        };

        grid.Children.Add(ellipse);
        return grid;
    }

    protected Grid CreateAntiControlDot()
    {
        var ellipse = new Ellipse()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Height = smallSize,
            Width = smallSize,
            Fill = orangeBrush,
            Stroke = blueBrush,
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

    protected Grid CreateHalfSwap()
    {
        var rectangle1 = new Rectangle()
        {
            Height = largeSize,
            Width = lineSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = blueBrush,
            Stroke = blueBrush,
            RenderTransform = new RotateTransform(angle: 45.0),
        };

        var rectangle2 = new Rectangle()
        {
            Height = lineSize,
            Width = largeSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = blueBrush,
            Stroke = blueBrush,
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

    protected Grid CreateCNot()
    {
        var ellipse = new Ellipse()
        {
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Height = largeSize,
            Width = largeSize,
            Stroke = blueBrush,
            StrokeThickness = lineSize,
            Fill = transparentBrush,
        };

        var rectangle1 = new Rectangle()
        {
            Height = largeSize,
            Width = lineSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = blueBrush,
        };

        var rectangle2 = new Rectangle()
        {
            Height = lineSize,
            Width = largeSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = blueBrush,
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

    public double GateHeight { get => this.Get<double>(); set => this.Set(value); }
}