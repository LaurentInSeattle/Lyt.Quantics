namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

using static GateUiConstants;
using static GateUiColors;

/// <summary> Abstract base class for Controlled and Constructed gates  </summary>
/// <typeparam name="TView">The corresponding view.</typeparam>
public abstract class CompositeGateViewModel<TView> : GateViewModelBase<TView>
        where TView : Control, new()
{
    protected readonly Grid contentGrid;
    protected readonly List<int> allQuBitIndicesSorted;

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
        double height = gateCount * GateSize + spacerCount * SpacerSize;
        this.GateHeight = height;
        var grid = new Grid()
        {
            Height = height,
            Width = GateSize,
#if DEBUG
            // ShowGridLines = true,
#endif
        };

        var border = new Border()
        {
            Margin = new Thickness(2.0),
            CornerRadius = new CornerRadius(4.0),
            Background = BackgroundBrush,
            VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        var rowDefinitions = new RowDefinitions
        {
            new RowDefinition(GateSize, GridUnitType.Pixel)
        };

        for (int i = 0; i < spacerCount; i++)
        {
            rowDefinitions.Add(new RowDefinition(SpacerSize, GridUnitType.Pixel));
            rowDefinitions.Add(new RowDefinition(GateSize, GridUnitType.Pixel));
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
        double height = gateCount * GateSize + spacerCount * SpacerSize - GateSize;
        var rectangle = new Rectangle()
        {
            Height = height,
            Width = ThinLineSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = BlueBrush,
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
            Height = DotSmallSize,
            Width = DotSmallSize,
            Fill = BlueBrush,
        };

        var grid = new Grid()
        {
            Height = GateSize,
            Width = GateSize,
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
            Height = DotSmallSize,
            Width = DotSmallSize,
            Fill = OrangeBrush,
            Stroke = BlueBrush,
            StrokeThickness = 2.0,
        };

        var grid = new Grid()
        {
            Height = GateSize,
            Width = GateSize,
        };

        grid.Children.Add(ellipse);
        return grid;
    }

    protected Grid CreateHalfSwap()
    {
        var rectangle1 = new Rectangle()
        {
            Height = LargeSize,
            Width = ThinLineSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = BlueBrush,
            Stroke = BlueBrush,
            RenderTransform = new RotateTransform(angle: 45.0),
        };

        var rectangle2 = new Rectangle()
        {
            Height = ThinLineSize,
            Width = LargeSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = BlueBrush,
            Stroke = BlueBrush,
            RenderTransform = new RotateTransform(angle: 45.0),
        };

        var grid = new Grid()
        {
            Height = GateSize,
            Width = GateSize,
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
            Height = LargeSize,
            Width = LargeSize,
            Stroke = BlueBrush,
            StrokeThickness = ThinLineSize,
            Fill = TransparentBrush,
        };

        var rectangle1 = new Rectangle()
        {
            Height = LargeSize,
            Width = ThinLineSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = BlueBrush,
        };

        var rectangle2 = new Rectangle()
        {
            Height = ThinLineSize,
            Width = LargeSize,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Fill = BlueBrush,
        };

        var grid = new Grid()
        {
            Height = GateSize,
            Width = GateSize,
        };

        grid.Children.Add(rectangle1);
        grid.Children.Add(rectangle2);
        grid.Children.Add(ellipse);
        return grid;
    }

    #endregion Gate Elements 

    public double GateHeight { get => this.Get<double>(); set => this.Set(value); }
}
