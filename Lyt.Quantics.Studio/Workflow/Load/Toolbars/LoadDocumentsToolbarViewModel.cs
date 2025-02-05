namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

public sealed class LoadDocumentsToolbarViewModel : Bindable<LoadDocumentsToolbarView> 
{
    private readonly QsModel quanticsStudioModel;

    public LoadDocumentsToolbarViewModel()
        => this.quanticsStudioModel = App.GetRequiredService<QsModel>();            

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.ShowMru = this.quanticsStudioModel.ShowRecentDocuments;
    } 

    public bool ShowMru
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.quanticsStudioModel.ShowRecentDocuments = value; 
        }
    }
}
