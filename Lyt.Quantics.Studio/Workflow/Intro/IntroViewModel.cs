namespace Lyt.Quantics.Studio.Workflow.Intro;

using static HeaderedContentViewModel;

public sealed partial class IntroViewModel : ViewModel<IntroView>
{
    private readonly DoIntroViewModel doIntroViewModel;

    [ObservableProperty]
    private HeaderedContentView doIntro;

    public IntroViewModel()
    {
        string title = "Q\u2009u\u2009a\u2009n\u2009t\u2009i\u2009c\u2009s     S\u2009t\u2009u\u2009d\u2009i\u2009o";
        this.DoIntro =
            CreateContent<DoIntroViewModel, DoIntroView, DoIntroToolbarViewModel, DoIntroToolbarView>(
                title, canCollapse: false);
        this.doIntroViewModel = this.DoIntro.ViewModel<DoIntroViewModel>();
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        this.doIntroViewModel.Activate(activationParameters);
    }
}
