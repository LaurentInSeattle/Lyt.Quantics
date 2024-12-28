namespace Lyt.Quantics.Studio.Workflow.Load;

using static ToolbarCommandMessage;

public sealed class LoadBuiltInViewModel : Bindable<LoadBuiltInView>
{
    private const string IsUnitTestProperty = "IsUnitTest";
    private const string NameProperty = "Name";
    private const string DescriptionProperty = "Description";

    private readonly List<FilterString> stringFilters;
    private readonly List<FilterPredicate> boolFilters;

    private SearchEngine<BuiltInViewModel>? searchEngine;
    private FilterPredicate? filterPredicate;

    private List<BuiltInViewModel> builtInViews;

    public LoadBuiltInViewModel()
    {
        this.builtInViews = [];
        this.stringFilters = [];
        this.boolFilters = [];
        this.filterPredicate = new(PropertyName: IsUnitTestProperty, PropertyValue: false);

        // Subscribtion processed locally 
        this.Messenger.Subscribe<ToolbarCommandMessage>(this.OnToolbarCommandMessage);
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.OnBuiltInClearSearch();
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        if ((this.builtInViews.Count == 0) || (this.searchEngine is null))
        {
            var builtInComputers = QsModel.BuiltInComputers;
            this.builtInViews = new(builtInComputers.Count);
            var computerNames = from key in builtInComputers.Keys orderby key select key;
            foreach (string computerName in computerNames)
            {
                try
                {
                    var computer = builtInComputers[computerName];
                    builtInViews.Add(new BuiltInViewModel(computer));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    this.Logger.Warning("Failed to load " + ex.ToString());
                    continue;
                }
            }

            this.searchEngine = new(this.builtInViews, this.Logger);
        }

        this.OnBuiltInClearSearch();
    }

    private void OnToolbarCommandMessage(ToolbarCommandMessage message)
    {
        switch (message.Command)
        {
            default: return;

            case ToolbarCommand.BuiltInClearSearch: this.OnBuiltInClearSearch(); break;

            case ToolbarCommand.BuiltInSearch: this.OnBuiltInSearch(message.CommandParameter); break;

            case ToolbarCommand.ShowRegular: this.OnShowRegular(message.CommandParameter); break;
        }
    }

    private void OnShowRegular(object? commandParameter)
    {
        if (commandParameter is not bool value)
        {
            return;
        }

        // value is true for regular, flip it to check regular (non UT) circuits 
        this.filterPredicate = new(PropertyName: IsUnitTestProperty, PropertyValue: !value);
        this.DoFilter();
    }

    private void OnBuiltInSearch(object? commandParameter)
    {
        if ((commandParameter is not List<string> searchTokens) || (searchTokens.Count == 0))
        {
            return;
        }

        this.stringFilters.Clear();
        foreach (string token in searchTokens)
        {
            this.stringFilters.Add(new (PropertyName: NameProperty, token));
            this.stringFilters.Add(new (PropertyName: DescriptionProperty, token));
        }

        this.DoFilter();
    }

    private void OnBuiltInClearSearch()
    {
        if (this.searchEngine is null)
        {
            return;
        }

        this.stringFilters.Clear();
        this.DoFilter();
    }

    private void DoFilter()
    {
        if (this.searchEngine is null)
        {
            return;
        }

        this.boolFilters.Clear();
        if (this.filterPredicate is not null)
        {
            this.boolFilters.Add(this.filterPredicate);
        }

        var filteredResults = this.searchEngine.Filter(this.stringFilters, this.boolFilters);
        if (filteredResults.Success)
        {
            this.BuiltInViews = filteredResults.Result;
        }
        else
        {
            this.BuiltInViews = this.searchEngine.All;
            this.Logger.Warning("Search failed: " + filteredResults.Message);
        }
    }

    public List<BuiltInViewModel> BuiltInViews
    {
        get => this.Get<List<BuiltInViewModel>>()!;
        set => this.Set(value);
    }
}
