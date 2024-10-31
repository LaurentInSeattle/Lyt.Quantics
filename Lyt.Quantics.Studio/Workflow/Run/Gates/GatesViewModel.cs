namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class GatesViewModel : Bindable<GatesView>
{
    public GatesViewModel()
    {
        this.DisablePropertyChangedLogging = true; 
        this.Messenger.Subscribe<GateHoverMessage>(this.OnGateHover);
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Load the gates symbols in the 'toolbox' 
        // Sort and reorder by categories skipping the X special ones
        var gates =
            (from gate in QuanticsStudioModel.Gates
             where (gate.Category != GateCategory.X_Special) && (gate.CaptionKey != "I")
             orderby gate.Category.ToString() ascending,
             gate.CaptionKey ascending
             select gate);
        var list = new List<GateViewModel>(gates.Count());
        foreach (var gate in gates)
        {
            list.Add(new GateViewModel(gate, isToolbox: true));
        }

        this.Gates = list;
    }

#pragma warning disable CA1822 // Mark members as static
    public bool CanDrop(Point _, GateViewModel gateViewModel) => !gateViewModel.IsToolbox;

    public void OnDrop(Point _, GateViewModel gateViewModel)
#pragma warning restore CA1822 
    {
        if (gateViewModel.IsToolbox)
        {
            return;
        }

        Debug.WriteLine("Gates View Model: OnDrop");
        gateViewModel.Remove();
    }

    private void OnGateHover(GateHoverMessage message)
    {
        if (message.IsEnter)
        {
            this.GateTitle = message.GateCaptionKey;
            this.GateDescription = "Yolo: List<GateViewModel>? Gates { get => this.Get<List<GateViewModel>?>(); set => this.Set(value); }";
        }
        else
        {
            this.GateTitle = string.Empty;
            this.GateDescription = string.Empty;
        }
    }

    public List<GateViewModel>? Gates { get => this.Get<List<GateViewModel>?>(); set => this.Set(value); }

    public string? GateTitle { get => this.Get<string?>(); set => this.Set(value); }

    public string? GateDescription { get => this.Get<string?>(); set => this.Set(value); }
}
