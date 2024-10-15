namespace Lyt.Quantics.Studio.Messaging;

public sealed class ModelStructureUpdateMessage(
    bool qubitsChanged, bool stageChanged = false, int indexStageChanged = -1)
{
    public bool QubitsChanged { get; private set; } = qubitsChanged;

    public bool StageChanged { get; private set; } = stageChanged;

    public int IndexStageChanged { get; private set; } = indexStageChanged;
}
