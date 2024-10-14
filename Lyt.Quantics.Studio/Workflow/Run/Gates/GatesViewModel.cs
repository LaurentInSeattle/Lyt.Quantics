namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class GatesViewModel : Bindable<GatesView> 
{
    private readonly QuanticsStudioModel quanticsStudioModel; 

    public GatesViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();    
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Load the gates symbols in the 'toolbox' 
        // Sort and reorder by categories 
        var gates = 
            (from gate in QuanticsStudioModel.Gates 
             where gate.Category != GateCategory.X_Special 
             orderby gate.Category.ToString() ascending,
             gate.CaptionKey ascending
             select gate);
        var list = new List<GateViewModel>(gates.Count());
        foreach (var gate in gates)
        {
            var vm = new GateViewModel(gate, isToolbox: true);
            list.Add(vm);
        }

        this.Gates = list;
    }

    public List<GateViewModel>? Gates { get => this.Get<List<GateViewModel>?>(); set => this.Set(value); }
}
