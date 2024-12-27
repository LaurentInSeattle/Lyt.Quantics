namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

using static ToolbarCommandMessage;
using static MessagingExtensions;

public sealed class LoadBuiltInToolbarViewModel : Bindable<LoadBuiltInToolbarView>
{
    public LoadBuiltInToolbarViewModel() { }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.View.FilterTextBox.Text = string.Empty;
    }

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable CA1822 // Mark members as static

    private void OnClearSearch(object? _) => Command(ToolbarCommand.BuiltInClearSearch);

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
            Command(ToolbarCommand.ShowRegular, value);
        }
    }

    public ICommand ClearSearchCommand { get => this.Get<ICommand>()!; set => this.Set(value); }
}
