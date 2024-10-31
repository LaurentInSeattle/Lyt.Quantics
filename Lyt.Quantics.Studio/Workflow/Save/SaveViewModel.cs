namespace Lyt.Quantics.Studio.Workflow.Save;

using static HeaderedContentViewModel;

public sealed class SaveViewModel : Bindable<SaveView>
{
    private readonly DoSaveViewModel doSaveViewModel;

    public SaveViewModel()
    {
        this.DoSave =
            CreateContent<DoSaveViewModel, DoSaveView, DoSaveToolbarViewModel, DoSaveToolbarView>(
                "Save your Project", canCollapse: false);
        this.doSaveViewModel = this.DoSave.ViewModel<DoSaveViewModel>();
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.doSaveViewModel.Activate(activationParameters);
        this.Messenger.Publish(new ShowTitleBarMessage(Show: false));
    }

    public HeaderedContentView DoSave
    {
        get => this.Get<HeaderedContentView>()!;
        set => this.Set(value);
    }
}
