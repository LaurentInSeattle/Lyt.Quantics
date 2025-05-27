namespace Lyt.Quantics.Studio.Workflow.Intro;

using static MessagingExtensions;
using static ViewActivationMessage;

public sealed partial class DoIntroToolbarViewModel : ViewModel<DoIntroToolbarView>
{
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnExit() => ActivateView(ActivatedView.Exit);

    [RelayCommand]
    public void OnNext() => ActivateView(ActivatedView.Load);

#pragma warning restore CA1822 // 

    // TODO: License: 
    // https://opensource.org/license/mit
}
