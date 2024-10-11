namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

public sealed class AmplitudesViewModel : Bindable<AmplitudesView>
{
    // TODO : 
    // Show only non zero / show all 
    // Show only above value 
    // Order by decreasing probability / by bit order 

    protected override void OnViewLoaded()
    {
        var vm = new HistogramViewModel();
        vm.CreateViewAndBind();
        this.View.AmplitudesGrid.Children.Add(vm.View);
    }
}
