﻿namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

using static GateUiConstants;
using static GateUiColors;

public sealed class GateViewModel : GateViewModelBase<GateView>
{
    public GateViewModel(
        Gate gate, 
        bool isGhost = false, bool isToolbox = false, int stageIndex = -1, 
        int qubitIndex = -1)
        : base ( gate, new QubitsIndices(qubitIndex), isGhost, isToolbox, stageIndex)

    {
        if (!this.IsGhost)
        {
            this.DragAble = new DragAble();
        }

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
        if (gate.HasAngleParameter)
        {
            this.FontSize -= 6;
            this.Parameter = gate.AngleParameterCaption;
            this.ParameterFontSize = 13.0;
        }


        this.IsBorderVisible = true;
        this.GateMargin = new Thickness(this.IsToolbox ? 10 : 0);
        int gateRows = this.Gate.MatrixDimension / 2;
        if (this.IsToolbox || (gateRows == 1))
        {
            this.GateBackground = Brushes.Black;
            int gateQubits = this.Gate.QuBitsTransformed;
            this.GateHeight = GateSize + 16 * (gateQubits - 1);
        }
        else
        {
            this.IsBorderVisible = false;
            this.GateBackground = Brushes.Transparent;
            this.GateHeight = GateSize * gateRows + SpacerSize * (gateRows - 1);
            Debug.WriteLine("GateHeight: " + this.GateHeight);
        }

        this.GateCategoryBrush = GateCategoryToBrush(gate.Category);
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

    // The special gates are here only for display in the toolbox
    // We use ConstructedGate in the circuit view 
    // CS is the exception that should disappear when Constructed Gate is able to 
    // display a more general Controlled Gate 
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

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        if (!this.IsGhost && this.DragAble is not null && !this.DragAble.IsAttached)
        {
            this.DragAble.Attach(this.View);
            this.View.InvalidateVisual();
        }
    }

    // IDraggableBindable Implementation 
    public override UserControl CreateGhostView()
    {
        var ghostViewModel = new GateViewModel(this.Gate, isGhost:true,  isToolbox: true);
        ghostViewModel.CreateViewAndBind();
        var view = ghostViewModel.View;

        // Create the special graphics if needed 
        if (GateViewModel.SpecialGateToControl(this.Gate.CaptionKey) is Control control)
        {
            view.GateIconContent.Content = control;
        }

        view.ZIndex = 999_999;
        view.Opacity = 0.8;
        view.InvalidateVisual();
        return view;
    }

    #region Bound properties 

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

    #endregion Bound properties 
}
