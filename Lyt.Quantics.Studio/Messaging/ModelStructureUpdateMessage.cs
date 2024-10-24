namespace Lyt.Quantics.Studio.Messaging;

public sealed record class ModelStructureUpdateMessage(
    bool QubitsChanged, 
    bool StagePacked = false, 
    bool StageChanged = false, 
    int IndexStageChanged = -1)
{
    public static ModelStructureUpdateMessage MakeQubitsChanged()
        => new(QubitsChanged: true);

    public static ModelStructureUpdateMessage MakeStagePacked() 
        => new(QubitsChanged:false, StagePacked:true);

    public static ModelStructureUpdateMessage MakeStageChanged(int indexStageChanged)
        => new(QubitsChanged: false, StagePacked: false, StageChanged: true, indexStageChanged);
}
