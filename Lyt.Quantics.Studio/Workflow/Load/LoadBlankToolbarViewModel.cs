namespace Lyt.Quantics.Studio.Workflow.Load;

using static Lyt.Quantics.Studio.Messaging.ViewActivationMessage;
using static Lyt.Quantics.Studio.Messaging.MessagingExtensions;

public sealed class LoadBlankToolbarViewModel : Bindable<LoadBlankToolbarView>
{
    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnCreateBlank(object? _)
        => ActivateView(
            ActivatedView.Run,
            new ComputerActivationParameter(ComputerActivationParameter.Kind.New));

#pragma warning restore CA1822 // 
#pragma warning restore IDE0051
    #endregion Methods invoked by the Framework using reflection 

    public ICommand CreateBlankCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
