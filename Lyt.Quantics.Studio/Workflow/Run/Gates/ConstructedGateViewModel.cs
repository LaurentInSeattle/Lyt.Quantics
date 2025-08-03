namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed partial class ConstructedGateViewModel : CompositeGateViewModel<ConstructedGateView>
{
    // Controlled Gates are not supported here, they have their own class  
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
        };

    // This is the actual key for the gate
    private readonly string gateKey;

    public ConstructedGateViewModel(
        string gateKey, int stageIndex, QubitsIndices qubitsIndices, bool isGhost = false)
        : base(GateFactory.Produce(gateKey, new GateParameters()),
            stageIndex, qubitsIndices, isGhost)
    {
        this.gateKey = gateKey;
        Rectangle rectangle = this.CreateConnectingLine();
        this.contentGrid.Children.Add(rectangle);
        this.CreateGate();
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();

        if (!this.IsGhost)
        {            
            this.DragAble = new DragAble(((MainWindow)App.MainWindow).MainWindowCanvas);
            this.DragAble.Attach(this.View);
            this.View.Content = this.contentGrid;
            this.View.InvalidateVisual();
        }
    }

    public static bool IsGateSupported(string gateKey)
        => ConstructedGateViewModel.supportedGates.ContainsKey(gateKey);

    // Draggable Bindable Implementation 
    public override View CreateGhostView()
    {
        var ghostViewModel = new ConstructedGateViewModel(
            this.gateKey, this.StageIndex, this.QubitsIndices, isGhost: true);
        ghostViewModel.CreateViewAndBind();
        var view = ghostViewModel.View;

        // Do NOT use 'this'.contentGrid (already has a parent and will crash Avalonia) 
        view.Content = ghostViewModel.contentGrid;
        view.ZIndex = 999_999;
        view.Opacity = 0.8;
        view.InvalidateVisual();
        return view;
    }

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
        var parameters = this.QubitsIndices;
        var control = this.CreateControlDot();
        int controlIndex = parameters.ControlQuBitIndices[0];
        this.PlaceGridAt(control, controlIndex);
        var not = this.CreateCNot();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(not, targetIndex);
    }

    private void CreateACxGate()
    {
        var parameters = this.QubitsIndices;
        var control = this.CreateAntiControlDot();
        int controlIndex = parameters.ControlQuBitIndices[0];
        this.PlaceGridAt(control, controlIndex);
        var not = this.CreateCNot();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(not, targetIndex);
    }

    private void CreateSwapGate()
    {
        var parameters = this.QubitsIndices;
        var first = this.CreateHalfSwap();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(first, targetIndex);
        var last = this.CreateHalfSwap();
        targetIndex = parameters.TargetQuBitIndices[1];
        this.PlaceGridAt(last, targetIndex);
    }

    private void CreateCzGate()
    {
        var parameters = this.QubitsIndices;
        var first = this.CreateControlDot();
        int targetIndex = parameters.TargetQuBitIndices[0];
        this.PlaceGridAt(first, targetIndex);
        var last = this.CreateControlDot();
        targetIndex = parameters.TargetQuBitIndices[1];
        this.PlaceGridAt(last, targetIndex);
    }

    private void CreateCCzGate()
    {
        var parameters = this.QubitsIndices;
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
        var parameters = this.QubitsIndices;
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
        var parameters = this.QubitsIndices;
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
}
