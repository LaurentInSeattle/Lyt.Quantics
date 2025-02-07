namespace Lyt.Quantics.Studio.Model.Messaging;

// No data for now
public sealed record class ModelResultsUpdateMessage;

public sealed record class ModelProgressMessage(bool IsComplete, int Step);
