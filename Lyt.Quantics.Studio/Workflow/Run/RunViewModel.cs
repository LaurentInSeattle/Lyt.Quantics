namespace Lyt.Quantics.Studio.Workflow.Run;

using static HeaderedContentViewModel;

public sealed class RunViewModel : Bindable<RunView>
{
    public RunViewModel() { }

    protected override void OnViewLoaded()
    {
        this.Messenger.Publish(new ShowTitleBarMessage(show: false));
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
