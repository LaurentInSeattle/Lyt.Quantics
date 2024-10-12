namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ToolbarCommandMessage;

public sealed class ComputerToolbarViewModel : Bindable<ComputerToolbarView>
{
    private void OnAddQubit(object? _) => this.Publish(ToolbarCommand.AddQubit);

    private void OnRemoveQubit(object? _) => this.Publish(ToolbarCommand.RemoveQubit);

    private void OnReset(object? _) => this.Publish(ToolbarCommand.Reset);

    private void OnStep(object? _) => this.Publish(ToolbarCommand.Step);

    private void OnRun(object? _) => this.Publish(ToolbarCommand.Run);

    private void Publish(ToolbarCommand command, object? parameter = null)
        => this.Messenger.Publish(new ToolbarCommandMessage(command, parameter));

    public ICommand AddQubitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand RemoveQubitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ResetCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand StepCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand RunCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public bool HideProbabilities
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.Publish(ToolbarCommand.HideProbabilities, value);
        }
    }

}
