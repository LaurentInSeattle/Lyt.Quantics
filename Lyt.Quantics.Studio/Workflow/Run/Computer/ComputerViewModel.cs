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

    protected override void OnViewLoaded()
    {
        Schedule.OnUiThread(
            5_000, () =>
            {

                this.toaster.Dismiss();
                this.toaster.Host = this.View.ToasterHost;
            }, DispatcherPriority.ApplicationIdle);
    }

    private void OnQubitChangedMessage(QubitChangedMessage message)
        => this.UpdateQubit(message.Index, message.InitialState);

    private void OnToolbarCommandMessage(ToolbarCommandMessage message)
    {
        int count = this.Qubits.Count;
        switch (message.Command)
        {
            case ToolbarCommand.AddQubit: this.AddQubit(count); break;

            case ToolbarCommand.RemoveQubit: this.RemoveQubit(count); break;

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

    private void UpdateQubit(int index, QuState newState)
    {
        if (( index <= ComputerViewModel.MaxQubits) || ( index < 0 ))
        {
            if (!this.quanticsStudioModel.UpdateQubit(index, newState, out string message))
            {
                this.toaster.Show("Failed to update Qubit!", message, 4_000, InformationLevel.Error);
            }
        }
        else
        {
            // message 
            // TODO: Still missing its icon ! 
            this.toaster.Show(
                string.Format("No such Qubit at index: {0}!", index),
                string.Format("Failed to correctly index Qubit at: {0}.", index),
                4_000, InformationLevel.Error);
        }
    }

    private void AddQubit(int count)
    {
        if (count < ComputerViewModel.MaxQubits)
        {
            if( this.quanticsStudioModel.AddQubit(count, out string message))
            {
                this.Qubits.Add(new QubitViewModel(count));
            }
            else
            {
                this.toaster.Show( "Failed to Add Qubit!", message, 4_000, InformationLevel.Error);
            }
        }
        else
        {
            // message 
            // TODO: Still missing its icon ! 
            this.toaster.Show(
                string.Format("Add Qubit: Max {0}!", MaxQubits),
                string.Format("This Quantum Computer implementation is limited to {0} Qubits...", MaxQubits),
                4_000, InformationLevel.Warning);
        }
    }

    private void RemoveQubit(int count)
    {
        if (count > 0)
        {
            if (this.quanticsStudioModel.RemoveQubit(count, out string message))
            {
                this.Qubits.RemoveAt(count - 1);
            }
            else
            {
                this.toaster.Show("Failed to Remove last Qubit!", message, 4_000, InformationLevel.Error);
            }
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
