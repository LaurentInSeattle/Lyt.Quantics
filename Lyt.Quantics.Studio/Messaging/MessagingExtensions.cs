namespace Lyt.Quantics.Studio.Messaging;

using static ToolbarCommandMessage;

public static class MessagingExtensions
{
    private static readonly IMessenger messenger;
    private static readonly IDialogService dialogService;

    static MessagingExtensions()
    {
        MessagingExtensions.messenger = App.GetRequiredService<IMessenger>();
        MessagingExtensions.dialogService = App.GetRequiredService<IDialogService>();
    }

    public static void ActivateView (
        ViewActivationMessage.ActivatedView view, object? activationParameter = null)
        => MessagingExtensions.messenger.Publish(
            new ViewActivationMessage(view, activationParameter));

    public static void Command(ToolbarCommand command, object? parameter = null)
    {
        // All toolbar messaging is disabled 
        // TODO: Visual State of toolbars
        if (MessagingExtensions.dialogService.IsModal)
        {
            return; 
        }

        MessagingExtensions.messenger.Publish(new ToolbarCommandMessage(command, parameter));
    }
}
