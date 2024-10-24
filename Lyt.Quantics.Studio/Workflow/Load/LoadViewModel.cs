namespace Lyt.Quantics.Studio.Workflow.Load;

using static HeaderedContentViewModel;

public sealed class LoadViewModel : Bindable<LoadView>
{
    public LoadViewModel()
    {
        SerializationUtilities.GetEmbeddedComputerNames(); 
    }

    protected override void OnViewLoaded()
    {
        this.Messenger.Publish(new ShowTitleBarMessage(Show: false));
        this.Blank =
            CreateContent<LoadBlankViewModel, LoadBlankView, LoadBlankToolbarViewModel, LoadBlankToolbarView>(
                "Start an Empty Blank New Project", canCollapse: false);

        this.BuiltIn =
            CreateContent<LoadBuiltInViewModel, LoadBuiltInView, LoadBuiltInViewModel, LoadBuiltInView>(
                "Ready to Use Built-in Projects", canCollapse: false);

        this.Documents =
            CreateContent<LoadDocumentsViewModel, LoadDocumentsView, LoadDocumentsViewModel, LoadDocumentsView>(
                "Your Previously Saved Projects", canCollapse: false);
    }

    public Control Blank { get => this.Get<Control>()!; set => this.Set(value); }

    public Control BuiltIn { get => this.Get<Control>()!; set => this.Set(value); }

    public Control Documents { get => this.Get<Control>()!; set => this.Set(value); }
}
