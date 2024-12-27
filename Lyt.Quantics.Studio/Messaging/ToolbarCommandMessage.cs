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

        // Documents 
        DeleteDocument, 

        // LAter
        Loop,

        // QuBit actions
        InitialQuStateChanged,
        Exit,

        // Amplitudes toolbar 
        ShowAll, 
        ShowByBitOrder,
        ShowStage,

        // Builtin Projects toolbar 
        BuiltInClearSearch,
        BuiltInSearch,
        ShowRegular,

        // Saved Projects toolbar 
        Mru,
    }
}
