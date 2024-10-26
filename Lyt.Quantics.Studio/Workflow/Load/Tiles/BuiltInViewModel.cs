namespace Lyt.Quantics.Studio.Workflow.Load.Tiles;

using static Lyt.Quantics.Studio.Messaging.ViewActivationMessage;
using static Lyt.Quantics.Studio.Messaging.MessagingExtensions;

public sealed class BuiltInViewModel : Bindable<BuiltInView>
{
    public BuiltInViewModel(QuComputer quComputer)
    {
        base.DisablePropertyChangedLogging = true;
        base.DisableAutomaticBindingsLogging = true; 

        this.Name = quComputer.Name;
        this.Description = quComputer.Description;
    }

    #region Methods invoked by the Framework using reflection 
#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnOpen(object? _)
        => ActivateView(
            ActivatedView.Run, 
            new ComputerActivationParameter(
                ComputerActivationParameter.Kind.Resource, this.Name));   

    private void OnDelete(object? _)
    {
        this.Logger.Info("Clicked on Delete!");
    }

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members
#pragma warning restore CA1822 // Mark members as static

    public ICommand OpenCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand DeleteCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Description { get => this.Get<string>()!; set => this.Set(value); }
}
