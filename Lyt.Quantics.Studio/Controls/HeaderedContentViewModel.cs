namespace Lyt.Quantics.Studio.Controls;

public sealed class HeaderedContentViewModel : Bindable<HeaderedContentView>
{
    private readonly bool canCollapse; 
    private readonly bool collapseLeft;

    public HeaderedContentViewModel(
        string title, bool canCollapse, bool collapseLeft, Control contentView, Control? toolbar)
    {
        this.canCollapse = canCollapse;
        this.collapseLeft = collapseLeft;
        this.Title = title;
        this.ContentView = contentView;
        this.Toolbar = toolbar;
        this.IsCollapseVisible = canCollapse;
        this.CollapseIconSource = this.collapseLeft ? "chevron_left" : "chevron_right";
        this.ExpandIconSource = this.collapseLeft ? "chevron_right" : "chevron_left";
        this.Collapse(collapse: false);
    }

    private void Collapse(bool collapse)
    {
        this.IsCollapsed = collapse;
        this.IsExpanded = !collapse;
    }

    private void OnCollapse(object? _) => this.Collapse(collapse: true);

    private void OnExpand(object? _) => this.Collapse(collapse: false);

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public string? CollapseIconSource { get => this.Get<string?>(); set => this.Set(value); }

    public string? ExpandIconSource { get => this.Get<string?>(); set => this.Set(value); }

    public bool IsCollapseVisible { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsCollapsed { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsExpanded { get => this.Get<bool>(); set => this.Set(value); }

    public ICommand CollapseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExpandCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public Control? Toolbar { get => this.Get<Control?>()!; set => this.Set(value); }

    public Control ContentView { get => this.Get<Control>()!; set => this.Set(value); }
}
