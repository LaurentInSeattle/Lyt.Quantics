namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

using static ToolbarCommandMessage;
using static MessagingExtensions;

public sealed class LoadBuiltInToolbarViewModel : Bindable<LoadBuiltInToolbarView>
{
    private readonly QsModel quanticsStudioModel;

    public LoadBuiltInToolbarViewModel()
        => this.quanticsStudioModel = App.GetRequiredService<QsModel>();


    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.View.FilterTextBox.Text = string.Empty;
        this.ShowRegular = this.quanticsStudioModel.ShowBuiltInComputers; 
    }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnClearSearch(object? _)
    {
        this.View.FilterTextBox.Text = string.Empty;
        Command(ToolbarCommand.BuiltInClearSearch);
    } 

#pragma warning restore CA1822 // Mark members as static
#pragma warning restore IDE0051 // Remove unused private members

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

    public bool ShowRegular
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            this.quanticsStudioModel.ShowBuiltInComputers = value; 
        }
    }

    public ICommand ClearSearchCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
