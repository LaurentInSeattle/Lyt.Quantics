namespace Lyt.Quantics.Studio.Workflow.Load;

using static ToolbarCommandMessage;

public sealed class LoadDocumentsViewModel : Bindable<LoadDocumentsView>
{
    private readonly QuanticsStudioModel quanticsStudioModel;
    private readonly List<DocumentViewModel> documentViews;

    private DocumentViewModel? documentToDelete;

    public LoadDocumentsViewModel()
    {
        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
        this.documentViews = [];
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommandMessage);
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

    private void OnToolbarCommandMessage(ToolbarCommandMessage message)
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
            this.DocumentViews.Remove(this.documentToDelete);
        }
        else
        {
            // TODO: Toast message 
        } 
    }

    public ObservableCollection<DocumentViewModel> DocumentViews
    {
        get => this.Get<ObservableCollection<DocumentViewModel>>()!;
        set => this.Set(value);
    }
}
