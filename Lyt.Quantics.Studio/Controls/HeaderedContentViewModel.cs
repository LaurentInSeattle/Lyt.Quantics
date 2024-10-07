namespace Lyt.Quantics.Studio.Controls; 

public sealed class HeaderedContentViewModel : Bindable<HeaderedContentView>
{
    public HeaderedContentViewModel(string title, Control contentView, Control? toolbar)
    {
        this.Collapse(collapse: false);
        this.Title = title;
        this.ContentView = contentView;
        this.Toolbar = toolbar;
    }

    private void Collapse(bool collapse)
    {
        this.IsCollapsed = collapse;
        this.IsExpanded = !collapse;
    } 

    private void OnCollapse(object? _) => this.Collapse(collapse:true);

    private void OnExpand(object? _) => this.Collapse(collapse: false);

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public bool IsCollapsed { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsExpanded { get => this.Get<bool>(); set => this.Set(value); }

    public ICommand CollapseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExpandCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public Control? Toolbar { get => this.Get<Control?>()!; set => this.Set(value); }

    public Control ContentView { get => this.Get<Control>()!; set => this.Set(value); }
}
