namespace Lyt.Quantics.Studio.Workflow.Load.Tiles;

public sealed class DocumentViewModel : Bindable<DocumentView>
{
    public DocumentViewModel()
    {
        base.DisablePropertyChangedLogging = true;
        base.DisableAutomaticBindingsLogging = true;
    }
}
