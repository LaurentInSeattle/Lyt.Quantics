namespace Lyt.Quantics.Studio.Messaging;

public sealed record class GateEditMessage(IGateInfoProvider GateInfoProvider, bool WithModifier);
