namespace Lyt.Quantics.Studio.Messaging;

public sealed record class ModelStructureUpdateMessage(
    bool QubitsChanged,
    bool ModelLoaded = false,
    bool StagePacked = false,
    bool StageChanged = false,
    int IndexStageChanged = -1)
{
    public static ModelStructureUpdateMessage MakeQubitsChanged()
        => new(QubitsChanged: true);

    public static ModelStructureUpdateMessage MakeModelLoaded()
        => new(QubitsChanged: false, ModelLoaded: true, StagePacked: false);

    public static ModelStructureUpdateMessage MakeStagePacked()
        => new(QubitsChanged: false, ModelLoaded: false, StagePacked: true);

    public static ModelStructureUpdateMessage MakeStageChanged(int indexStageChanged)
        => new(QubitsChanged: false, ModelLoaded: false, StagePacked: false, StageChanged: true, indexStageChanged);
}
