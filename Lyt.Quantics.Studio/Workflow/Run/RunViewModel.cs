namespace Lyt.Quantics.Studio.Workflow.Run;

using static HeaderedContentViewModel;

public sealed class RunViewModel : Bindable<RunView>
{
    public RunViewModel() { }

    protected override void OnViewLoaded()
    {
        static Control CreateContent<TViewModel, TControl, TToolbarViewModel, TToolbarControl>(
            string title, bool canCollapse,
            CollapseStyle collapseStyle = CollapseStyle.Left,
            bool createCollapsed = false)
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
                new HeaderedContentViewModel(title, canCollapse, baseVm.View, toolbarVm.View, collapseStyle);
            headerVm.CreateViewAndBind();
            if (createCollapsed)
            {
                headerVm.Collapse(true);
            }

            return headerVm.View;
        }

        this.Gates =
            CreateContent<GatesViewModel, GatesView, GatesToolbarViewModel, GatesToolbarView>(
                "Gates", canCollapse: true, CollapseStyle.Left);

        // TODO: Figure out if this is really needed and useful
        //
        // Hiding this for now 
        //this.Code =
        //    CreateContent<CodeViewModel, CodeView, CodeToolbarViewModel, CodeToolbarView>(
        //        "JSon Code", canCollapse: true, CollapseStyle.Right, createCollapsed: true);

        this.Computer =
            CreateContent<ComputerViewModel, ComputerView, ComputerToolbarViewModel, ComputerToolbarView>(
                "Quantum Computer", canCollapse: false);

        this.Amplitudes =
            CreateContent<AmplitudesViewModel, AmplitudesView, AmplitudesToolbarViewModel, AmplitudesToolbarView>(
                "Amplitudes Histogram", canCollapse: true, CollapseStyle.Bottom, createCollapsed: true);
    }

    public Control Gates { get => this.Get<Control>()!; set => this.Set(value); }

    public Control Computer { get => this.Get<Control>()!; set => this.Set(value); }

    public Control Amplitudes { get => this.Get<Control>()!; set => this.Set(value); }

    // TODO: Figure out if this is really needed and useful
    //
    // Hiding this for now 
    // public Control Code { get => this.Get<Control>()!; set => this.Set(value); }
}
