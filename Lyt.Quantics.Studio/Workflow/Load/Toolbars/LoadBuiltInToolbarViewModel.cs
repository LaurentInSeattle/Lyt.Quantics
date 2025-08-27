namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

using static ToolbarCommandMessage;
using static ApplicationMessagingExtensions;

public sealed partial class LoadBuiltInToolbarViewModel : ViewModel<LoadBuiltInToolbarView>
{
    private readonly QsModel quanticsStudioModel;

    [ObservableProperty]
    public bool showRegular;

    public LoadBuiltInToolbarViewModel()
        => this.quanticsStudioModel = App.GetRequiredService<QsModel>();


    public override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.View.FilterTextBox.Text = string.Empty;
        this.ShowRegular = this.quanticsStudioModel.ShowBuiltInComputers; 
    }

    [RelayCommand]
    public void OnClearSearch()
    {
        this.View.FilterTextBox.Text = string.Empty;
        Command(ToolbarCommand.BuiltInClearSearch);
    } 

    public void OnEditing()
    {
        if (this.View is null)
        {
            return;
        }

        string? filter = this.View.FilterTextBox.Text;
        if (string.IsNullOrWhiteSpace(filter) || (filter.Length <= 2))
        {
            return;
        }

        string[] tokens = filter.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        List<string> filters = new(tokens.Length);
        foreach (string token in tokens)
        {
            if (!string.IsNullOrWhiteSpace(token) && token.Length >= 3)
            {
                filters.Add(token);
            }
        }

        if (filters.Count > 0)
        {
            Command(ToolbarCommand.BuiltInSearch, filters);
        }
    }

    partial void OnShowRegularChanged ( bool value)
            => this.quanticsStudioModel.ShowBuiltInComputers = value; 
}
