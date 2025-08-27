namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

using static ApplicationMessagingExtensions;

public sealed partial class LoadBlankToolbarViewModel : ViewModel<LoadBlankToolbarView>
{
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnCreateBlank()
        => Select(
            ActivatedView.Run,
            new ComputerActivationParameter(ComputerActivationParameter.Kind.New));

#pragma warning restore CA1822 // 
}
