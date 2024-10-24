namespace Lyt.Quantics.Studio.Workflow.Load;

using static Lyt.Quantics.Studio.Messaging.ViewActivationMessage;
using static Lyt.Quantics.Studio.Messaging.MessagingExtensions;
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
            CreateContent<LoadBlankViewModel, LoadBlankView, LoadBlankViewModel, LoadBlankView>(
                "Start an Empty Blank New Project", canCollapse: false);

        this.BuiltIn =
            CreateContent<LoadBuiltInViewModel, LoadBuiltInView, LoadBuiltInViewModel, LoadBuiltInView>(
                "Ready to Use Built-in Projects", canCollapse: false);

        this.Documents =
            CreateContent<LoadDocumentsViewModel, LoadDocumentsView, LoadDocumentsViewModel, LoadDocumentsView>(
                "Your Previously Saved Projects", canCollapse: false);
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnCreateBlank(object? _) 
        => ActivateView(
            ActivatedView.Run, 
            new ComputerActivationParameter(ComputerActivationParameter.Kind.New));

#pragma warning restore CA1822 // 
#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    public Control Blank { get => this.Get<Control>()!; set => this.Set(value); }

    public Control BuiltIn { get => this.Get<Control>()!; set => this.Set(value); }

    public Control Documents { get => this.Get<Control>()!; set => this.Set(value); }

    public ICommand CreateBlankCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
