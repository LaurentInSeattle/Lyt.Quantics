namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

using static ToolbarCommandMessage;
using static MessagingExtensions;

public sealed class LoadDocumentsToolbarViewModel : Bindable<LoadDocumentsToolbarView> 
{
    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.ShowMru = false;
    } 

    public bool ShowMru
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            Command(ToolbarCommand.Mru, value);
        }
    }
}
