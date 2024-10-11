using Lyt.Quantics.Studio.Controls.Histogram;

namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

public sealed class AmplitudesViewModel : Bindable<AmplitudesView>
{
    protected override void OnViewLoaded()
    {
        var vm = new HistogramViewModel();
        vm.CreateViewAndBind();
        this.View.AmplitudesGrid.Children.Add( vm.View );
    }
}
