﻿namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ToolbarCommandMessage;
using static MessagingExtensions; 

public sealed class ComputerToolbarViewModel : Bindable<ComputerToolbarView>
{
    private void OnAddQubit(object? _) => Command(ToolbarCommand.AddQubit);

    private void OnRemoveQubit(object? _) => Command(ToolbarCommand.RemoveQubit);

    private void OnPackStages(object? _) => Command(ToolbarCommand.PackStages);

    private void OnReset(object? _) => Command(ToolbarCommand.Reset);

    private void OnStep(object? _) => Command(ToolbarCommand.Step);

    private void OnRun(object? _) => Command(ToolbarCommand.Run);

    private void OnSave(object? _) => Command(ToolbarCommand.Save);

    private void OnClose(object? _) => Command(ToolbarCommand.Close);

    public ICommand AddQubitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand RemoveQubitCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand PackStagesCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ResetCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand StepCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand RunCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public bool HideProbabilities
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            Command(ToolbarCommand.HideProbabilities, value);
        }
    }

}
