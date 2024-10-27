namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

using static ToolbarCommandMessage;
using static MessagingExtensions;

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
            Command(ToolbarCommand.ShowAll, value);
        }
    }

    public bool ShowByBitOrder
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            Command(ToolbarCommand.ShowByBitOrder, value);
        }
    }
}
