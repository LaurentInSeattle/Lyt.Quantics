namespace Lyt.Quantics.Studio.Workflow.Intro;

using static MessagingExtensions;
using static ViewActivationMessage;

public sealed class DoIntroToolbarViewModel : Bindable<DoIntroToolbarView>
{
    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnExit(object? _) => ActivateView(ActivatedView.Exit);

    private void OnNext(object? _) => ActivateView(ActivatedView.Load);

#pragma warning restore CA1822 // 
#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    // TODO: License: 
    // https://opensource.org/license/mit

    public ICommand ExitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand NextCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
