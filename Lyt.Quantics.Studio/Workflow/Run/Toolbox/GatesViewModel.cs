namespace Lyt.Quantics.Studio.Workflow.Run.Toolbox;

public sealed class GatesViewModel : Bindable<GatesView>
{
    private readonly QsModel quanticsStudioModel;
    private readonly IToaster toaster;

    public GatesViewModel()
    {
        this.DisablePropertyChangedLogging = true;

        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = ApplicationBase.GetRequiredService<QsModel>();
        this.toaster = ApplicationBase.GetRequiredService<IToaster>();
        this.Messenger.Subscribe<GateHoverMessage>(this.OnGateHover);
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Load the gates symbols in the 'toolbox' 
        // Sort and reorder by categories skipping identity and the X special ones
        var gates =
            from gate in QsModel.Gates
            where gate.Category != GateCategory.Special && gate.CaptionKey != "I"
            orderby (int) gate.Category ascending,
            gate.CaptionKey ascending
            select gate;
        var list = new List<GateViewModel>(gates.Count());
        foreach (var gate in gates)
        {
            list.Add(new GateViewModel(gate, isToolbox: true));
        }

        this.Gates = list;
    }

#pragma warning disable CA1822 // Mark members as static
    public bool CanDrop(Point _, IGateInfoProvider gateInfoProvider)
        => !gateInfoProvider.IsToolbox;
#pragma warning restore CA1822 

    public void OnDrop(Point _, IGateInfoProvider gateInfoProvider)
    {
        if (gateInfoProvider.IsToolbox)
        {
            return;
        }

        Debug.WriteLine("Gates View Model: OnDrop");
        this.Remove(gateInfoProvider);
    }

    private void OnGateHover(GateHoverMessage message)
    {
        if (message.IsEnter)
        {
            this.GateTitle = message.GateCaptionKey;
            this.GateDescription = message.GateDescription;
        }
        else
        {
            this.GateTitle = string.Empty;
            this.GateDescription = string.Empty;
        }
    }

    private void Remove(IGateInfoProvider gateInfoProvider)
    {
        Debug.WriteLine("Removing gate: " + gateInfoProvider.Gate.CaptionKey);
        if (!this.quanticsStudioModel.RemoveGate(
            gateInfoProvider.StageIndex, gateInfoProvider.QubitsIndices, out string message))
        {
            this.toaster.Show(
                "Failed to Remove gate: " + gateInfoProvider.Gate.CaptionKey, message,
                4_000, InformationLevel.Error);
        }
    }

    public List<GateViewModel>? Gates { get => this.Get<List<GateViewModel>?>(); set => this.Set(value); }

    public string? GateTitle { get => this.Get<string?>(); set => this.Set(value); }

    public string? GateDescription { get => this.Get<string?>(); set => this.Set(value); }
}
