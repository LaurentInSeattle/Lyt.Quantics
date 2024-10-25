namespace Lyt.Quantics.Studio.Workflow.Run;

using static HeaderedContentViewModel;

public sealed class RunViewModel : Bindable<RunView>
{
    private ComputerViewModel computerViewModel; 

    public RunViewModel() => this.computerViewModel = new();

    protected override void OnViewLoaded()
    {
        this.Messenger.Publish(new ShowTitleBarMessage(Show: false));
        this.Gates =
            CreateContent<GatesViewModel, GatesView, GatesToolbarViewModel, GatesToolbarView>(
                "Gates", canCollapse: true, CollapseStyle.Left);

        this.Computer =
            CreateContent<ComputerViewModel, ComputerView, ComputerToolbarViewModel, ComputerToolbarView>(
                "Quantum Computer", canCollapse: false);
        this.computerViewModel = this.Computer.ViewModel<ComputerViewModel>();

        this.Amplitudes =
            CreateContent<AmplitudesViewModel, AmplitudesView, AmplitudesToolbarViewModel, AmplitudesToolbarView>(
                "Amplitudes Histogram", canCollapse: true, CollapseStyle.Bottom, createCollapsed: true);

        // TODO: Figure out if this is really needed and useful
        //
        // Hiding this for now 
        //this.Code =
        //    CreateContent<CodeViewModel, CodeView, CodeToolbarViewModel, CodeToolbarView>(
        //        "JSon Code", canCollapse: true, CollapseStyle.Right, createCollapsed: true);
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.computerViewModel.Activate(activationParameters);
    } 

    public HeaderedContentView Gates { get => this.Get<HeaderedContentView>()!; set => this.Set(value); }

    public HeaderedContentView Computer { get => this.Get<HeaderedContentView>()!; set => this.Set(value); }

    public HeaderedContentView Amplitudes { get => this.Get<HeaderedContentView>()!; set => this.Set(value); }

    // TODO: Figure out if this is really needed and useful
    //
    // Hiding this for now 
    // public Control Code { get => this.Get<Control>()!; set => this.Set(value); }
}
