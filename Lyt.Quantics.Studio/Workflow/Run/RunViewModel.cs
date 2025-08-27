namespace Lyt.Quantics.Studio.Workflow.Run;

using static HeaderedContentViewModel;

public sealed partial class RunViewModel : ViewModel<RunView>
{
    private readonly ComputerViewModel computerViewModel;

    [ObservableProperty]
    private HeaderedContentView gates;

    [ObservableProperty]
    private HeaderedContentView computer;

    [ObservableProperty]
    private HeaderedContentView amplitudes;

    public RunViewModel()
    {
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
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.computerViewModel.Activate(activationParameters);
        new ShowTitleBarMessage(Show: false).Publish();
    }

    #region Code View: Commented out for now 
    //protected override void OnViewLoaded()
    //{
    //    // TODO: Figure out if this is really needed and useful
    //    //
    //    // Hiding this for now 
    //    //this.Code =
    //    //    CreateContent<CodeViewModel, CodeView, CodeToolbarViewModel, CodeToolbarView>(
    //    //        "JSon Code", canCollapse: true, CollapseStyle.Right, createCollapsed: true);
    //}

    // TODO: Figure out if this is really needed and useful
    //
    // Hiding this for now 
    // public Control Code { get => this.Get<Control>()!; set => this.Set(value); }
    #endregion Code View: Commented out for now 
}
