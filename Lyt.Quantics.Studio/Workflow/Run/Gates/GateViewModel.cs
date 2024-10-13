namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class GateViewModel : Bindable<GateView> // : IDraggable
{
    public const string CustomDragAndDropFormat = "GateViewModel";

    private readonly Gate gate;

    public GateViewModel(Gate gate)
    {
        this.DisablePropertyChangedLogging = true;
        this.gate = gate;
        this.Name = gate.CaptionKey.Replace("dg", "\u2020");
        this.FontSize = Name.Length switch
        {
            1 => 30.0,
            2 => 30.0,
            3 => 20.0,
            4 => 15.0,
            _ => 13.0,
        };

        this.GateCategoryBrush = GateViewModel.GateCategoryToBrush(gate.Category);
    }

    private static IBrush GateCategoryToBrush(GateCategory gateCategory)
    {
        return gateCategory switch
        {
            GateCategory.A_HadamardAndT => Brushes.DarkOrange,
            GateCategory.B_Pauli => Brushes.DodgerBlue,
            GateCategory.C_Phase => Brushes.MediumAquamarine,
            GateCategory.D_BinaryControlled => Brushes.DarkGreen,
            GateCategory.E_Other => Brushes.DarkGray,
            GateCategory.F_TernaryControlled => Brushes.MediumPurple,
            /* default */
            _ => Brushes.DarkRed,
        };
    }

    public bool BeginDrag()
    {
        return true;
    }

    public string? Name { get => this.Get<string?>(); set => this.Set(value); }

    public double FontSize { get => this.Get<double>(); set => this.Set(value); }

    public IBrush? GateCategoryBrush { get => this.Get<IBrush?>(); set => this.Set(value); }
}
