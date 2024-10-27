namespace Lyt.Quantics.Studio.Messaging;

using static ToolbarCommandMessage;

public static class MessagingExtensions
{
    private static readonly IMessenger messenger; 

    static MessagingExtensions()
        => MessagingExtensions.messenger = App.GetRequiredService<IMessenger>();

    public static void ActivateView (
        ViewActivationMessage.ActivatedView view, object? activationParameter = null)
        => MessagingExtensions.messenger.Publish(
            new ViewActivationMessage(view, activationParameter));

    public static void Command(ToolbarCommand command, object? parameter = null)
        => MessagingExtensions.messenger.Publish(
            new ToolbarCommandMessage(command, parameter));

}
