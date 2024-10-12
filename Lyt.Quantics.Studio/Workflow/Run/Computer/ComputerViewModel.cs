namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ToolbarCommandMessage;

public sealed class ComputerViewModel : Bindable<ComputerView>
{
    public const int MaxQubits = 8; // For now ~ 10 could be doable ? 

    public ComputerViewModel()
    {
        this.Qubits = new ObservableCollection<QubitViewModel>(); 
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommandMessage);
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
                    // TODO : message 
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
