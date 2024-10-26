namespace Lyt.Quantics.Studio.Workflow.Load;

using static FileManagerModel; 

public sealed class LoadDocumentsViewModel : Bindable<LoadDocumentsView>
{
    private readonly List<DocumentViewModel> documentViews = [];

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.DocumentViews = documentViews;
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        try
        {
            this.documentViews.Clear();
            var fileManager = App.GetRequiredService<FileManagerModel>();
            var files = fileManager.Enumerate(Area.User, Kind.Json);
            foreach (string file in files)
            {
                try
                {
                    var computer = fileManager.Load<QuComputer>(Area.User, Kind.Json, file);
                    if (computer is null)
                    {
                        throw new Exception("Failed to deserialize");
                    }

                    bool isValid = computer.Validate(out string message);
                    if (!isValid)
                    {
                        throw new Exception(message);
                    }

                    bool isBuilt = computer.Build(out message);
                    if (!isBuilt)
                    {
                        throw new Exception(message);
                    }

                    var documentView = new DocumentViewModel();
                    documentViews.Add(documentView);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    this.Logger.Warning(file + " :  failed to load \n" + ex.ToString());
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

    public List<DocumentViewModel> DocumentViews
    {
        get => this.Get<List<DocumentViewModel>>()!; set => this.Set(value);    
    }
}
