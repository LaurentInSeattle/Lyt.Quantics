namespace Lyt.Quantics.Studio.Workflow.Intro;

using static MessagingExtensions;

public sealed partial class DoIntroToolbarViewModel : ViewModel<DoIntroToolbarView>
{
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnExit() => ShellViewModel.OnExit();

    [RelayCommand]
    public void OnNext() => Select(ActivatedView.Load);

#pragma warning restore CA1822 // 
}
