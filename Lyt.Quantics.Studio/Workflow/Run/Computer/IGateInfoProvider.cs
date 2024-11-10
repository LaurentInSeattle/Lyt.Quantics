namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public interface IGateInfoProvider
{
    Gate Gate { get; }

    /// <summary> True when this is a ghost gate view model. </summary>
    bool IsGhost { get; }

    /// <summary> True when this is a toolbox gate view model. </summary>
    bool IsToolbox { get; }

    /// <summary> Only valid when this is NOT a toolbox gate view model. </summary>
    int StageIndex { get; }

    /// <summary> Only valid when this is NOT a toolbox gate view model. </summary>
    int QubitIndex { get;}
}
