namespace Lyt.Quantics.Studio.Messaging;

public sealed record class GateHoverMessage(bool IsEnter = false, string GateCaptionKey = "");
