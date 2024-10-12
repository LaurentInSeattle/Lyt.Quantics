namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ToolbarCommandMessage;

public sealed class ComputerViewModel : Bindable<ComputerView>
{
    public const int MaxQubits = 10; // For now ~ 10 could be doable ? 

    private readonly QuanticsStudioModel quanticsStudioModel;
    private readonly IToaster toaster;

    public ComputerViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.toaster = App.GetRequiredService<IToaster>();
        this.Qubits = [];
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommandMessage);
        this.Messenger.Subscribe<QubitChangedMessage>(this.OnQubitChangedMessage);
    }

    private void OnQubitChangedMessage(QubitChangedMessage message)
    {

    }

    private void OnToolbarCommandMessage(ToolbarCommandMessage message)
    {
        int count = this.Qubits.Count;
        switch (message.Command)
        {
            case ToolbarCommand.AddQubit:
                if (count < ComputerViewModel.MaxQubits)
                {
                    this.Qubits.Add(new QubitViewModel(count));
                } 
                else
                {
                    // message 
                    // TODO: Still missing its icon ! 
                    this.toaster.Show(
                        string.Format("Max {0} Qubits!", MaxQubits),
                        string.Format("This Quantum Computer implementation is limited to {0} Qubits...", MaxQubits),
                        4_000, InformationLevel.Warning);
                }
                break;

            case ToolbarCommand.RemoveQubit:
                if (count > 0)
                {
                    this.Qubits.RemoveAt(count-1);
                }

                break;
            case ToolbarCommand.HideProbabilities:
                break;
            case ToolbarCommand.Reset:
                break;
            case ToolbarCommand.Step:
                break;
            case ToolbarCommand.Run:
                break;
            case ToolbarCommand.Loop:
                break;
            case ToolbarCommand.Exit:
                break;
            default:
                break;
        }
    }

    public bool CanDrop(Point point)
    {
        // TODO
        return point.X > point.Y;
    }

    public void OnDrop(Point point, GateViewModel gateViewModel)
    {
        // TODO
        Debug.WriteLine("ComputerViewModel: OnDrop");
    }

    public ObservableCollection<QubitViewModel> Qubits 
    { 
        get => this.Get<ObservableCollection<QubitViewModel>>()!; 
        set => this.Set(value); 
    }
}
