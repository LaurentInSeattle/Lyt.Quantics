namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

using static Lyt.Quantics.Studio.Messaging.ToolbarCommandMessage;

public sealed class AmplitudesToolbarViewModel : Bindable<AmplitudesToolbarView>
{
    protected override void OnViewLoaded()
    {
        this.ShowAll = true;
        this.ShowByBitOrder = true;
    }

    public bool ShowAll
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.Publish(ToolbarCommand.ShowAll, value);
        }
    }

    public bool ShowByBitOrder
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.Publish(ToolbarCommand.ShowByBitOrder, value);
        }
    }

    private void Publish(ToolbarCommand command, object? parameter = null)
        => this.Messenger.Publish(new ToolbarCommandMessage(command, parameter));
}
