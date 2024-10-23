namespace Lyt.Quantics.Studio.Messaging;

public sealed class ModelStructureUpdateMessage(
    bool qubitsChanged, 
    bool stagePacked = false, 
    bool stageChanged = false, 
    int indexStageChanged = -1)
{
    public bool QubitsChanged { get; private set; } = qubitsChanged;

    public bool StagePacked { get; private set; } = stagePacked;

    public bool StageChanged { get; private set; } = stageChanged;

    public int IndexStageChanged { get; private set; } = indexStageChanged;

    public static ModelStructureUpdateMessage MakeQubitsChanged()
        => new(qubitsChanged: true);

    public static ModelStructureUpdateMessage MakeStagePacked() 
        => new(qubitsChanged:false, stagePacked:true);

    public static ModelStructureUpdateMessage MakeStageChanged(int indexStageChanged)
        => new(qubitsChanged: false, stagePacked: false, stageChanged: true, indexStageChanged);

}
