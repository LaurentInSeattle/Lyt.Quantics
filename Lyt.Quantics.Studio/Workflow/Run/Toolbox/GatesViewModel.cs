namespace Lyt.Quantics.Studio.Workflow.Run.Toolbox;

public sealed partial class GatesViewModel : ViewModel<GatesView>, IRecipient<GateHoverMessage>
{
    private readonly QsModel quanticsStudioModel;
    private readonly IToaster toaster;

    [ObservableProperty]
    private List<GateViewModel>? gates;

    [ObservableProperty]
    private string? gateTitle;

    [ObservableProperty]
    private string? gateDescription;

    public GatesViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = ApplicationBase.GetRequiredService<QsModel>();
        this.toaster = ApplicationBase.GetRequiredService<IToaster>();
        this.Subscribe<GateHoverMessage>();
    }

    public override void OnViewLoaded()
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

    public void Receive(GateHoverMessage message)
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
}
