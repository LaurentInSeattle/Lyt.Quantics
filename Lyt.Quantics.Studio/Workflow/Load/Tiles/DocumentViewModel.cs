namespace Lyt.Quantics.Studio.Workflow.Load.Tiles;

using static ToolbarCommandMessage;
using static ApplicationMessagingExtensions;

public sealed partial class DocumentViewModel : ViewModel<DocumentView>
{
    private readonly QuComputer quComputer;

    [ObservableProperty]
    public partial string Name { get; set; }

    [ObservableProperty]
    public partial string Opened { get; set; }

    [ObservableProperty]
    public partial string Description { get; set; }

    public DocumentViewModel(QuComputer quComputer)
    {
        this.quComputer = quComputer;
        this.Name = quComputer.Name;
        this.Description = quComputer.Description;
        var lastOpened = quComputer.LastOpened;
        this.LastOpened = lastOpened; 
        this.Opened = 
            string.Concat (lastOpened.ToShortDateString(), "  ", lastOpened.ToShortTimeString());
        this.IsRecent = (DateTime.Now - this.quComputer.LastOpened) < TimeSpan.FromDays(3);
    }

    public QuComputer QuComputer => this.quComputer;

    public bool IsRecent { get; private set; }

    // Replicates the model property for sorting purpose 
    public DateTime LastOpened { get; private set; }

    [RelayCommand]
    public void OnOpen(object? _)
        => Select(
            ActivatedView.Run,
            new ComputerActivationParameter(
                ComputerActivationParameter.Kind.Document, string.Empty, this.QuComputer));

    [RelayCommand]
    public void OnDelete(object? _)
        => Command(ToolbarCommand.DeleteDocument, this);
}
