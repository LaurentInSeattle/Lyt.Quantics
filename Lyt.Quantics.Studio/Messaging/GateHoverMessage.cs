namespace Lyt.Quantics.Studio.Messaging;

public sealed class GateHoverMessage(bool isEnter = false, string gateCaptionKey = "")
{
    public bool IsEnter { get; private set; } = isEnter;

    public string GateCaptionKey { get; private set; } = gateCaptionKey;
}
