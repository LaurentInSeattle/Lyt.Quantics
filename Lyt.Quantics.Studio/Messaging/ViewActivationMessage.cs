namespace Lyt.Quantics.Studio.Messaging;

public sealed record class ViewActivationMessage(
    ViewActivationMessage.ActivatedView View, object? ActivationParameter = null)
{
    public enum ActivatedView
    {
        Intro,
        Load, 
        Run,

        GoBack,
        Exit,
    }
}
