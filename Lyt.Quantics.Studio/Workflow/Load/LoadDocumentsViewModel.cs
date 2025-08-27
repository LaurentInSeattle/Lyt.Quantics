namespace Lyt.Quantics.Studio.Workflow.Load;

using static ToolbarCommandMessage;

public sealed partial class LoadDocumentsViewModel : 
    ViewModel<LoadDocumentsView>,
    IRecipient<ToolbarCommandMessage>,
    IRecipient<ModelUpdateMessage>
{
    private readonly QsModel quanticsStudioModel;
    private readonly List<DocumentViewModel> loadedDocumentViews;

    [ObservableProperty]
    private string? noData;

    [ObservableProperty]
    private ObservableCollection<DocumentViewModel> documentViews;
    
    private SearchEngine<DocumentViewModel>? searchEngine;
    private DocumentViewModel? documentToDelete;

    public LoadDocumentsViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the LoadView 
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
        this.loadedDocumentViews = [];
        this.DocumentViews = [];
        this.Subscribe<ToolbarCommandMessage>();
        this.Subscribe<ModelUpdateMessage>();
    }

    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.ShowMostRecent(this.quanticsStudioModel.ShowRecentDocuments);
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        try
        {
            this.loadedDocumentViews.Clear();
            var projects = this.quanticsStudioModel.Projects;
            if (projects.Count > 0)
            {
                this.NoData = string.Empty;
                var computerNames = from key in projects.Keys orderby key select key;
                foreach (var computerName in computerNames)
                {
                    try
                    {
                        var computer = projects[computerName];
                        var documentView = new DocumentViewModel(computer);
                        loadedDocumentViews.Add(documentView);
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex);
                        this.Logger.Warning(computerName + " :  failed to load \n" + ex.ToString());
                        continue;
                    }
                }
            }
            else
            {
                this.NoData = "< No saved projects were found. >";
            }

            this.searchEngine = new(this.loadedDocumentViews, this.Logger);
            this.ShowMostRecent(this.quanticsStudioModel.ShowRecentDocuments);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            this.Logger.Warning("One or more files failed to load \n" + ex.ToString());
        }
    }

    public void Receive(ModelUpdateMessage message)
    {
        if (message.PropertyName == nameof(QsModel.ShowRecentDocuments))
        {
            this.ShowMostRecent(this.quanticsStudioModel.ShowRecentDocuments);
        }
    }

    public void Receive(ToolbarCommandMessage message)
    {
        switch (message.Command)
        {
            case ToolbarCommand.DeleteDocument:
                if (message.CommandParameter is DocumentViewModel documentViewModel)
                {
                    this.OnDelete(documentViewModel);
                }

                break;

            default:
                break;
        }
    }

    private void OnDelete(DocumentViewModel documentViewModel)
    {
        var loadView = MiscUtilities.FindParentControl<LoadView>(this.View);
        if (loadView is null)
        {
            this.Logger.Warning("Parent view unreacheable!");
            return;
        }

        var panel = loadView.ModalHost;
        if (panel is null)
        {
            this.Logger.Warning("Parent view modal panel unreacheable!");
            return;
        }

        this.documentToDelete = documentViewModel;
        string name = this.documentToDelete.Name;
        var dialogService = App.GetRequiredService<IDialogService>();
        var confirmActionParameters = new ConfirmActionParameters
        {
            Title = "Confirm Delete",
            Message =
            string.Format(
                "This quantum computer file ({0}) is about to be permanently deleted.", name),
            ActionVerb = "Delete",
            OnConfirm = this.OnDeleteConfirmed,
            InformationLevel = InformationLevel.Warning,
        };

        dialogService.Confirm(panel, confirmActionParameters);
    }

    private void OnDeleteConfirmed(bool confirmed)
    {
        if ((!confirmed) || (this.documentToDelete is null))
        {
            // just dismiss
            return;
        }

        // Delete file and update UI 
        if (this.quanticsStudioModel.DeleteDocument(this.documentToDelete.QuComputer, out string message))
        {
            this.loadedDocumentViews.Remove(this.documentToDelete);
            this.searchEngine = new(this.loadedDocumentViews, this.Logger);
            this.ShowMostRecent(this.quanticsStudioModel.ShowRecentDocuments);
        }
        else
        {
            // TODO: Toast message 
            Debug.WriteLine(message);
        }
    }

    private void ShowMostRecent(bool mostRecent)
    {
        if (this.searchEngine is null)
        {
            return;
        }

        if (mostRecent)
        {
            var filter =
                new FilterPredicate(PropertyName: "IsRecent", PropertyValue: true);
            var sort = 
                new FilterSort(PropertyName: "LastOpened", IsAscending: false);
            var searchResults = this.searchEngine.Filter([filter], [sort]);
            if (searchResults.Success)
            {
                var result = searchResults.Result;
                if (result.Count < 3)
                {
                    // If nothing or almost nothing found, redo the search without the predicate
                    searchResults = this.searchEngine.Filter([], [], [sort]);

                    // and then take the top 5
                    result = [.. searchResults.Result.Take(5)];
                }

                this.DocumentViews = [.. result];
            }
            else
            {
                this.Logger.Warning("Search failed: " + searchResults.Message);
                this.DocumentViews = [.. this.searchEngine.All];
            }
        }
        else
        {
            this.DocumentViews = [.. this.searchEngine.All];
        }
    }
}
