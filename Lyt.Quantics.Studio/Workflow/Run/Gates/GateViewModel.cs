
namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class GateViewModel : Bindable<GateView> // : IDraggable
{
    public const string CustomDragAndDropFormat = "GateViewModel";

    private readonly QuanticsStudioModel quanticsStudioModel;
    private readonly IToaster toaster;

    public GateViewModel(
        Gate gate, bool isToolbox = false, int stageIndex = -1, int qubitIndex = -1)
    {
        // Too many properties here and too many gates !
        this.DisablePropertyChangedLogging = true;

        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.toaster = App.GetRequiredService<IToaster>();

        this.Gate = gate;
        this.IsToolbox = isToolbox;
        this.StageIndex = stageIndex;
        this.QubitIndex = qubitIndex;
        this.Name = gate.CaptionKey.Replace("dg", "\u2020");
        this.FontSize = Name.Length switch
        {
            1 => 30.0,
            2 => 30.0,
            3 => 20.0,
            4 => 15.0,
            _ => 13.0,
        };

        this.Parameter = string.Empty;
        this.ParameterFontSize = 1.0;
        if (gate.IsParametrized)
        {
            this.FontSize -= 6;
            this.Parameter = gate.ParameterCaption;
            this.ParameterFontSize = 12.0;
        }


        this.IsBorderVisible = true;
        this.GateMargin = new Thickness(this.IsToolbox ? 10 : 0);
        int gateRows = this.Gate.MatrixDimension / 2;
        if (this.IsToolbox || (gateRows == 1))
        {
            this.GateBackground = Brushes.Black;
            int gateQubits = this.Gate.QuBitsTransformed;
            this.GateHeight = 48 + 16 * (gateQubits - 1);
        }
        else
        {
            this.IsBorderVisible = false;
            this.GateBackground = Brushes.Transparent;
            int rowMargin = 8 + 4;
            this.GateHeight = 48 * gateRows + rowMargin * (gateRows - 1);
            Debug.WriteLine("GateHeight: " + this.GateHeight);
        }

        this.GateCategoryBrush = GateViewModel.GateCategoryToBrush(gate.Category);
        var gateControl = GateViewModel.SpecialGateToControl(gate.CaptionKey);
        if (gateControl is Control control)
        {
            this.SpecialGate = control;
            this.IsTextVisible = false;
            this.IsSpecialVisible = true;
        }
        else
        {
            this.IsTextVisible = true;
            this.IsSpecialVisible = false;
        }

        var disableOnModal = new DisabledOnModal();
        disableOnModal.Attach(this);
    }

    private static IBrush GateCategoryToBrush(GateCategory gateCategory)
        => gateCategory switch
        {
            GateCategory.A_HadamardAndT => Brushes.DarkOrange,
            GateCategory.B_Pauli => Brushes.DodgerBlue,
            GateCategory.C_Phase => Brushes.MediumAquamarine,
            GateCategory.D_Rotation => Brushes.DarkOrchid,
            GateCategory.E_BinaryControlled => Brushes.DarkGreen,
            GateCategory.F_Other => Brushes.DarkGray,
            GateCategory.G_TernaryControlled => Brushes.MediumPurple,
            /* default */
            _ => Brushes.DarkRed,
        };

    public static Control? SpecialGateToControl(string gateCaptionKey)
        => gateCaptionKey switch
        {
            "ACX" => new ACxGate(),
            "CX" => new CxGate(),
            "CCX" => new CCxGate(),
            "FCX" => new FCxGate(),
            "CZ" => new CzGate(),
            "CCZ" => new CCzGate(),
            "CS" => new CsGate(),
            "Swap" => new Special.SwapGate(),
            "CSwap" => new Special.CSwapGate(),
            /* default */
            _ => null,
        };

    /// <summary> True when this is a toolbox gate view model. </summary>
    public bool IsToolbox { get; private set; }

    /// <summary> Only valid when this is a toolbox gate view model. </summary>
    public int StageIndex { get; private set; }

    /// <summary> Only valid when this is a toolbox gate view model. </summary>
    public int QubitIndex { get; private set; }

    public Gate Gate { get; private set; }

    public void Remove()
    {
        Debug.WriteLine("Removing gate: " + this.Gate.CaptionKey);
        if (!this.quanticsStudioModel.RemoveGate(
            this.StageIndex, this.QubitIndex, this.Gate, out string message))
        {
            this.toaster.Show(
                "Failed to Remove gate: " + this.Gate.CaptionKey, message,
                4_000, InformationLevel.Error);
        }
    }

#pragma warning disable CA1822 // Mark members as static
    public bool BeginDrag() => true;  // For now 
#pragma warning restore CA1822 

    public void OnGateEntered()
    {
        if (!this.IsToolbox)
        {
            return;
        }

        this.Messenger.Publish(new GateHoverMessage(IsEnter: true, this.Gate.CaptionKey));
    }

    public void OnGateExited() => this.Messenger.Publish(new GateHoverMessage());

    public void OnGateClicked()
    {
        if (this.IsToolbox || !this.Gate.IsParametrized)
        {
            return;
        }

        // Launch edit gate dialog 
        this.Messenger.Publish(new GateEditMessage(this));
    }

    public double GateHeight { get => this.Get<double>(); set => this.Set(value); }

    public Thickness GateMargin { get => this.Get<Thickness>(); set => this.Set(value); }

    public string? Name { get => this.Get<string?>(); set => this.Set(value); }

    public double FontSize { get => this.Get<double>(); set => this.Set(value); }

    public string? Parameter { get => this.Get<string?>(); set => this.Set(value); }

    public double ParameterFontSize { get => this.Get<double>(); set => this.Set(value); }

    public IBrush? GateCategoryBrush { get => this.Get<IBrush?>(); set => this.Set(value); }

    public IBrush? GateBackground { get => this.Get<IBrush?>(); set => this.Set(value); }

    public bool IsTextVisible { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsBorderVisible
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.GateBorderThickness = new Thickness(value ? 1.0 : 0.0);
        }
    }

    public Thickness GateBorderThickness { get => this.Get<Thickness>(); set => this.Set(value); }

    public bool IsSpecialVisible { get => this.Get<bool>(); set => this.Set(value); }

    public Control SpecialGate { get => this.Get<Control>()!; set => this.Set(value); }
}
