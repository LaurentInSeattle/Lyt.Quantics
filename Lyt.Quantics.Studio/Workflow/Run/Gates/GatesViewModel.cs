namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class GatesViewModel : Bindable<GatesView> 
{
    private readonly QuanticsStudioModel quanticsStudioModel; 

    public GatesViewModel()
    {
        // Do not use Injection directly as this is loaded as a collection 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();    
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        // Load the gates symbols in the 'toolbox' 
        // TODO: Sort and reorder by categories 
        var gates = QuanticsStudioModel.Gates;
        var list = new List<GateViewModel>(gates.Count);
        foreach (var gate in gates)
        {
            var vm = new GateViewModel(gate);
            list.Add(vm);
        }

        this.Gates = list;
    }

    public List<GateViewModel>? Gates { get => this.Get<List<GateViewModel>?>(); set => this.Set(value); }
}
