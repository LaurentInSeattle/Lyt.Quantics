namespace Lyt.Quantics.Studio.Workflow.Save;

using static ToolbarCommandMessage;
using static MessagingExtensions;
using static ViewActivationMessage;

public sealed class DoSaveToolbarViewModel: Bindable<DoSaveToolbarView>
{
#pragma warning disable CA1822 // Mark members as static
#pragma warning disable IDE0051 // Remove unused private members

    // Autogenearted bindings
    private void OnSave(object? _) => Command(ToolbarCommand.SaveToFile, false);

    private void OnOverwrite(object? _) => Command(ToolbarCommand.SaveToFile, true);

    private void OnClose(object? _)
        => ActivateView(
            ActivatedView.Run,
            new ComputerActivationParameter(ComputerActivationParameter.Kind.Back));

#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore CA1822 // Mark members as static

    public ICommand OverwriteCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
