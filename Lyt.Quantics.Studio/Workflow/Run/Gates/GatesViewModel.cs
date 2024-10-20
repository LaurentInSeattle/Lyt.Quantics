namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class GatesViewModel : Bindable<GatesView> 
{
    private readonly QuanticsStudioModel quanticsStudioModel;
    private readonly IToaster toaster;

    public GatesViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.toaster = App.GetRequiredService<IToaster>();
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Load the gates symbols in the 'toolbox' 
        // Sort and reorder by categories skipping the X special ones
        var gates = 
            (from gate in QuanticsStudioModel.Gates 
             where (gate.Category != GateCategory.X_Special) && ( gate.CaptionKey != "I")
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

    public bool CanDrop(Point _, GateViewModel gateViewModel) => !gateViewModel.IsToolbox;

    public void OnDrop(Point _, GateViewModel gateViewModel)
    {
        if (gateViewModel.IsToolbox)
        {
            return;
        }

        Debug.WriteLine("Gates View Model: OnDrop");
        gateViewModel.Remove();
    }

    public List<GateViewModel>? Gates { get => this.Get<List<GateViewModel>?>(); set => this.Set(value); }
}
