namespace Lyt.Quantics.Studio.Workflow.Load.Tiles;

using static ViewActivationMessage;
using static MessagingExtensions;

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

    private void OnOpen(object? _)
        => ActivateView(
            ActivatedView.Run, 
            new ComputerActivationParameter(
                ComputerActivationParameter.Kind.Resource, this.Name));   

    #endregion Methods invoked by the Framework using reflection 
#pragma warning restore IDE0051 // Remove unused private members

    public ICommand OpenCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Description { get => this.Get<string>()!; set => this.Set(value); }
}
