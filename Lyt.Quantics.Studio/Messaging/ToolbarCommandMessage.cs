namespace Lyt.Quantics.Studio.Messaging;

public sealed record class ToolbarCommandMessage(
    ToolbarCommandMessage.ToolbarCommand Command, object? CommandParameter = null)
{
    public enum ToolbarCommand
    {
        // Computer tool bar 
        AddQubit,
        RemoveQubit,
        PackStages,
        HideProbabilities, 
        Reset,
        Step,
        Run,
        Save,
        SaveToFile,
        Close, 

        // LAter
        Loop,

        // QuBit actions
        InitialQuStateChanged,
        Exit,

        // Amplitudes toolbar 
        ShowAll, 
        ShowByBitOrder,
    }
}
