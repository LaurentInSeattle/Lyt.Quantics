namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public sealed class GatesViewModel : Bindable<GatesView> 
{
    public GatesViewModel()
    {
        
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        var vm = new GateViewModel();
        vm.CreateViewAndBind();
        var view = vm.View;
        view.SetValue(Grid.RowProperty, 1);
        view.SetValue(Grid.ColumnProperty, 1);
        this.View.MainGrid.Children.Add(view); 
    } 
}
