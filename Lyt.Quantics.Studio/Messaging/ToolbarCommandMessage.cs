namespace Lyt.Quantics.Studio.Messaging;

public sealed class ToolbarCommandMessage(
    ToolbarCommandMessage.ToolbarCommand command, object? commandParameter = null)
{
    public enum ToolbarCommand
    {
        // Computer tool bar 
        AddQubit,
        RemoveQubit, 
        HideProbabilities, 
        Reset,
        Step,
        Run,
        Loop,

        // QuBit actions
        InitialQuStateChanged,
        Exit,
    }

    public ToolbarCommand Command { get; private set; } = command;

    public object? CommandParameter { get; private set; } = commandParameter;
}
