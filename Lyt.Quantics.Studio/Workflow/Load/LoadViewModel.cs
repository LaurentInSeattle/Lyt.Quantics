namespace Lyt.Quantics.Studio.Workflow.Load;

using static HeaderedContentViewModel;

public sealed class LoadViewModel : Bindable<LoadView>
{
    private readonly LoadBuiltInViewModel loadBuiltInViewModel;

    private readonly LoadDocumentsViewModel loadDocumentsViewModel;

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
        this.Messenger.Publish(new ShowTitleBarMessage(Show: false));
    }

    public HeaderedContentView Blank { get => this.Get<HeaderedContentView>()!; set => this.Set(value); }

    public HeaderedContentView BuiltIn { get => this.Get<HeaderedContentView>()!; set => this.Set(value); }

    public HeaderedContentView Documents { get => this.Get<HeaderedContentView>()!; set => this.Set(value); }
}
