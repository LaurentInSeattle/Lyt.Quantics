namespace Lyt.Quantics.Studio.Workflow.Intro;

using static HeaderedContentViewModel;

public sealed class IntroViewModel: Bindable<IntroView>
{
    private readonly DoIntroViewModel doIntroViewModel;

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

    public HeaderedContentView DoIntro { get => this.Get<HeaderedContentView>()!; set => this.Set(value); }

    //    #region Methods invoked by the Framework using reflection 
    //#pragma warning disable IDE0051 // Remove unused private members
    //#pragma warning disable CA1822 // Mark members as static

    //    private void OnExit(object? _) => ActivateView(ActivatedView.Exit);

    //    private void OnNext(object? _) => ActivateView(ActivatedView.Load);

    //#pragma warning restore CA1822 // 
    //#pragma warning restore IDE0051
    //    #endregion Methods invoked by the Framework using reflection 

    //    // TODO: License: 
    //    // https://opensource.org/license/mit

    //    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    //    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
