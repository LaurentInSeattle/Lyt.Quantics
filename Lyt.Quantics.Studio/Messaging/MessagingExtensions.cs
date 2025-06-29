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

    public static void Select(ActivatedView activatedView, object? parameter = null)
        => ViewSelector<ActivatedView>.Select(
            MessagingExtensions.messenger, activatedView, parameter);

    public static void Exit() => ShellViewModel.OnExit();

    public static void Command(ToolbarCommand command, object? parameter = null)
    {
        // All toolbar messaging is disabled, visual state of the toolbar is handled with
        // another type of message, provided by the Modal Dialog management in the framework
        if (MessagingExtensions.dialogService.IsModal)
        {
            return;
        }

        MessagingExtensions.messenger.Publish(new ToolbarCommandMessage(command, parameter));
    }
}
