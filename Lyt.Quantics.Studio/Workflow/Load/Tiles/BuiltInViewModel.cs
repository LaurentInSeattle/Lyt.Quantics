namespace Lyt.Quantics.Studio.Workflow.Load.Tiles;

using static ApplicationMessagingExtensions;

public sealed partial class BuiltInViewModel : ViewModel<BuiltInView>
{
    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string description;

    public BuiltInViewModel(QuComputer quComputer)
    {
        this.IsUnitTest = quComputer.IsUnitTest; 
        this.Name = quComputer.Name;
        this.Description = quComputer.Description;
    }

    public bool IsUnitTest { get; private set; }

    [RelayCommand]
    public void OnOpen(object? _)
        => Select(
            ActivatedView.Run, 
            new ComputerActivationParameter(
                ComputerActivationParameter.Kind.Resource, this.Name));
}
