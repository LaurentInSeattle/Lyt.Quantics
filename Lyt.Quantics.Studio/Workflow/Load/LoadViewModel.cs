namespace Lyt.Quantics.Studio.Workflow.Load;

using static HeaderedContentViewModel;

public sealed class LoadViewModel : Bindable<LoadView>
{
    public LoadViewModel() 
    {
        this.Blank =
            CreateContent<LoadBlankViewModel, LoadBlankView, LoadBlankToolbarViewModel, LoadBlankToolbarView>(
                "Start an Empty Blank New Project", canCollapse: false);

        this.BuiltIn =
            CreateContent<LoadBuiltInViewModel, LoadBuiltInView, LoadBuiltInToolbarViewModel, LoadBuiltInToolbarView>(
                "Ready to Use Built-in Projects", canCollapse: false);

        this.Documents =
            CreateContent<LoadDocumentsViewModel, LoadDocumentsView, LoadDocumentsToolbarViewModel, LoadDocumentsToolbarView>(
                "Your Saved Projects", canCollapse: true, CollapseStyle.Right, createCollapsed: true);
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.Messenger.Publish(new ShowTitleBarMessage(Show: false));
    }

    public Control Blank { get => this.Get<Control>()!; set => this.Set(value); }

    public Control BuiltIn { get => this.Get<Control>()!; set => this.Set(value); }

    public Control Documents { get => this.Get<Control>()!; set => this.Set(value); }
}
