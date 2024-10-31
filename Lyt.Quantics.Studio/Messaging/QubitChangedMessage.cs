namespace Lyt.Quantics.Studio.Messaging;

public sealed record class QubitChangedMessage(int Index, QuState InitialState);