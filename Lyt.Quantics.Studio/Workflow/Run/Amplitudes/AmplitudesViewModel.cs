namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

public sealed class AmplitudesViewModel : Bindable<AmplitudesView>
{
    public AmplitudesViewModel()
    {
        this.Messenger.Subscribe<ModelResultsUpdateMessage>(this.OnModelResultsUpdateMessage);
    }

    // TODO : 
    // Show only non zero / show all 
    // Show only above value 
    // Order by decreasing probability / by bit order 

    protected override void OnViewLoaded()
    {
        var vm = new HistogramViewModel();        
        this.View.AmplitudesGrid.Children.Add(vm.CreateViewAndBind());
    }

    private void OnModelResultsUpdateMessage(ModelResultsUpdateMessage message)
    {
        // TODO
        // Update probabilities 
    }

}
