namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

using static Lyt.Quantics.Studio.Messaging.ViewActivationMessage;
using static Lyt.Quantics.Studio.Messaging.MessagingExtensions;

public sealed partial class LoadBlankToolbarViewModel : ViewModel<LoadBlankToolbarView>
{
#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnCreateBlank()
        => ActivateView(
            ActivatedView.Run,
            new ComputerActivationParameter(ComputerActivationParameter.Kind.New));

#pragma warning restore CA1822 // 
}
