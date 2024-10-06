namespace Lyt.Quantics.Studio.Messaging;

public sealed class ViewActivationMessage(ViewActivationMessage.ActivatedView view, object? activationParameter = null)
{
    public enum ActivatedView
    {
        Intro,
        Load, 
        Save, 
        Run,
        GoBack,
        Exit,
    }

    public ActivatedView View { get; private set; } = view;

    public object? ActivationParameter { get; private set; } = activationParameter;
}
