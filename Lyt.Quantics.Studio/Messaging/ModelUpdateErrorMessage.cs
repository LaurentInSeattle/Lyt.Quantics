namespace Lyt.Quantics.Studio.Messaging;

public sealed class ModelUpdateErrorMessage(string message)
{
    public string Message { get; private set; } = message;
}
