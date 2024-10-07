namespace Lyt.Quantics.Studio.Workflow.Run;

public sealed class RunViewModel : Bindable<RunView>
{
    public RunViewModel()
    {
    }

    protected override void OnViewLoaded()
    {
        var gatesVm = new GatesViewModel();
        gatesVm.CreateViewAndBind();
        var headerGatesVm = new HeaderedContentViewModel("Gates", gatesVm.View, null);
        headerGatesVm.CreateViewAndBind();
        this.Gates = headerGatesVm.View;
    }

    public Control Gates { get => this.Get<Control>()!; set => this.Set(value); }
}
