namespace Lyt.Quantics.Studio.Workflow.Run;

public sealed class RunViewModel : Bindable<RunView>
{
    public RunViewModel()
    {
    }

    protected override void OnViewLoaded()
    {
        Control CreateContent<TViewModel, TControl, TToolbarViewModel, TToolbarControl>(
            string title, bool canCollapse, bool collapseLeft)
            where TViewModel : Bindable<TControl>, new()
            where TControl : Control, new()
            where TToolbarViewModel : Bindable<TToolbarControl>, new()
            where TToolbarControl : Control, new()

        {
            var baseVm = new TViewModel();
            baseVm.CreateViewAndBind();
            var toolbarVm = new TToolbarViewModel();
            toolbarVm.CreateViewAndBind();
            var headerVm =
                new HeaderedContentViewModel(title, canCollapse, collapseLeft, baseVm.View, toolbarVm.View);
            headerVm.CreateViewAndBind();
            return headerVm.View;
        }

        this.Gates =
            CreateContent<GatesViewModel, GatesView, GatesToolbarViewModel, GatesToolbarView>(
                "Gates", canCollapse: true, collapseLeft: true);

        this.Code =
            CreateContent<CodeViewModel, CodeView, CodeToolbarViewModel, CodeToolbarView>(
                "JSon Code", canCollapse: true, collapseLeft: false);

        this.Computer =
            CreateContent<ComputerViewModel, ComputerView, ComputerToolbarViewModel, ComputerToolbarView>(
                "Quantum Computer", canCollapse: false, collapseLeft: false);

        this.Histogram =
            CreateContent<HistogramViewModel, HistogramView, HistogramToolbarViewModel, HistogramToolbarView>(
                "Histogram", canCollapse: false, collapseLeft: false);
    }

    public Control Gates { get => this.Get<Control>()!; set => this.Set(value); }

    public Control Code { get => this.Get<Control>()!; set => this.Set(value); }

    public Control Computer { get => this.Get<Control>()!; set => this.Set(value); }

    public Control Histogram { get => this.Get<Control>()!; set => this.Set(value); }
}
