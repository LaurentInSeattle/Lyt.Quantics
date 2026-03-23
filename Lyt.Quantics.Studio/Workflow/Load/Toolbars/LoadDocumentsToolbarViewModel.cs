namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

public sealed partial class LoadDocumentsToolbarViewModel : ViewModel<LoadDocumentsToolbarView> 
{
    private readonly QsModel quanticsStudioModel;

    [ObservableProperty]
    public partial bool ShowMru { get; set; }

    public LoadDocumentsToolbarViewModel()
        => this.quanticsStudioModel = App.GetRequiredService<QsModel>();            

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.ShowMru = this.quanticsStudioModel.ShowRecentDocuments;
    }

    partial void OnShowMruChanged(bool value ) 
        => this.quanticsStudioModel.ShowRecentDocuments = value; 
}
