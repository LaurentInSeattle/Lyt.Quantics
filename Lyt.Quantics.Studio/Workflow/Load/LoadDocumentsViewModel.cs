namespace Lyt.Quantics.Studio.Workflow.Load;

public sealed class LoadDocumentsViewModel : Bindable<LoadDocumentsView>
{
    private readonly QuanticsStudioModel quanticsStudioModel;
    private readonly List<DocumentViewModel> documentViews;

    public LoadDocumentsViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.documentViews = [];
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.DocumentViews = new(documentViews);
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);
        try
        {
            this.documentViews.Clear();
            foreach (var computer in this.quanticsStudioModel.Projects.Values)
            {
                try
                {
                    var documentView = new DocumentViewModel(computer);
                    documentViews.Add(documentView);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    this.Logger.Warning(computer.Name + " :  failed to load \n" + ex.ToString());
                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            this.Logger.Warning("One or more files failed to load \n" + ex.ToString());
        }
    }

    public ObservableCollection<DocumentViewModel> DocumentViews
    {
        get => this.Get<ObservableCollection<DocumentViewModel>>()!;
        set => this.Set(value);
    }
}
