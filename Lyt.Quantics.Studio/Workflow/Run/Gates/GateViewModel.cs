namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class GateViewModel : Bindable<GateView> // : IDraggable
{
    public const string CustomDragAndDropFormat = "GateViewModel";
    
    private readonly Gate gate;

    public GateViewModel(Gate gate)
    {
        this.gate = gate;
        this.Name = gate.CaptionKey;
        this.FontSize = gate.CaptionKey.Length switch
        {
            1 => 48,
            2 => 38,
            3 => 28,
            4 => (double)20,
            _ => (double)16,
        };
    }

    public bool BeginDrag() 
    { 
        return true;
    }

    public string? Name { get => this.Get<string?>(); set => this.Set(value); }

    public double FontSize { get => this.Get<double>(); set => this.Set(value); }
}
