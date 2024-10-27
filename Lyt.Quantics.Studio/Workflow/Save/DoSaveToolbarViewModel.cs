namespace Lyt.Quantics.Studio.Workflow.Save;

using static ToolbarCommandMessage;
using static MessagingExtensions;
using static ViewActivationMessage;

public sealed class DoSaveToolbarViewModel: Bindable<DoSaveToolbarView>
{
    private void OnSave(object? _) => Command(ToolbarCommand.Save);

    private void OnClose(object? _)
        => ActivateView(
            ActivatedView.Run, 
            new ComputerActivationParameter(ComputerActivationParameter.Kind.Back)); 
    
    public ICommand SaveCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand CloseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

}
