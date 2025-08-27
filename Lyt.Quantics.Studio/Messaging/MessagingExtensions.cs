namespace Lyt.Quantics.Studio.Messaging;

using static ToolbarCommandMessage;

public static class ApplicationMessagingExtensions
{
    private static readonly IDialogService dialogService;

    static ApplicationMessagingExtensions()
    {
        ApplicationMessagingExtensions.dialogService = App.GetRequiredService<IDialogService>();
    }

    public static void Select(ActivatedView activatedView, object? parameter = null)
        => ViewSelector<ActivatedView>.Select(activatedView, parameter);

    public static void Exit() => ShellViewModel.OnExit();

    public static void Command(ToolbarCommand command, object? parameter = null)
    {
        // All toolbar messaging is disabled, visual state of the toolbar is handled with
        // another type of message, provided by the Modal Dialog management in the framework
        if (ApplicationMessagingExtensions.dialogService.IsModal)
        {
            return;
        }

        new ToolbarCommandMessage(command, parameter).Publish();
    }
}
