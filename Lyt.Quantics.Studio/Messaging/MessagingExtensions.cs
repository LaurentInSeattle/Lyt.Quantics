namespace Lyt.Quantics.Studio.Messaging;

public static class MessagingExtensions
{
    private static readonly IMessenger messenger; 

    static MessagingExtensions()
        => MessagingExtensions.messenger = App.GetRequiredService<IMessenger>();

    public static void ActivateView (
        ViewActivationMessage.ActivatedView view, object? activationParameter = null)
        => MessagingExtensions.messenger.Publish(
            new ViewActivationMessage(view, activationParameter));
}
