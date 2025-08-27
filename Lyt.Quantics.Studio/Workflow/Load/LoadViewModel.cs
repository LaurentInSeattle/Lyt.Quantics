namespace Lyt.Quantics.Studio.Workflow.Load;

using static HeaderedContentViewModel;

public sealed partial class LoadViewModel : ViewModel<LoadView>
{
    private readonly LoadBuiltInViewModel loadBuiltInViewModel;
    private readonly LoadDocumentsViewModel loadDocumentsViewModel;

    [ObservableProperty]
    private HeaderedContentView blank;

    [ObservableProperty]
    private HeaderedContentView builtIn;

    [ObservableProperty]
    private HeaderedContentView documents;

    public LoadViewModel() 
    {
        this.Blank =
            CreateContent<LoadBlankViewModel, LoadBlankView, LoadBlankToolbarViewModel, LoadBlankToolbarView>(
                "Blank New Project", canCollapse: false);

        this.BuiltIn =
            CreateContent<LoadBuiltInViewModel, LoadBuiltInView, LoadBuiltInToolbarViewModel, LoadBuiltInToolbarView>(
                "Built-in Projects", canCollapse: false);
        this.loadBuiltInViewModel = this.BuiltIn.ViewModel<LoadBuiltInViewModel>();

        this.Documents =
            CreateContent<LoadDocumentsViewModel, LoadDocumentsView, LoadDocumentsToolbarViewModel, LoadDocumentsToolbarView>(
                "Saved Projects", canCollapse: true, CollapseStyle.Right, createCollapsed: false);
        this.loadDocumentsViewModel = this.Documents.ViewModel<LoadDocumentsViewModel>();
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.loadBuiltInViewModel.Activate(activationParameters);
        this.loadDocumentsViewModel.Activate(activationParameters);
        new ShowTitleBarMessage(Show: true).Publish();
    }
}
