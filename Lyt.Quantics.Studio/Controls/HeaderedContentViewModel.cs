namespace Lyt.Quantics.Studio.Controls;

public sealed partial class HeaderedContentViewModel : ViewModel<HeaderedContentView>
{
    public enum CollapseStyle
    {
        Left,
        Right,
        Top,
        Bottom,
    }

    private readonly CollapseStyle collapseStyle;

    [ObservableProperty]
    private string? title;

    [ObservableProperty]
    private string? collapseIconSource;

    [ObservableProperty]
    private string? expandIconSource;

    [ObservableProperty]
    private bool isCollapseVisible;

    [ObservableProperty]
    private bool isSideCollapsed;

    [ObservableProperty]
    private bool isUpdownCollapsed;

    [ObservableProperty]
    private bool isExpanded;

    [ObservableProperty]
    private Control? toolbar;

    [ObservableProperty]
    private Control contentView;

    public HeaderedContentViewModel(
        string title, bool canCollapse,
        Control contentView, Control? toolbar,
        CollapseStyle collapseStyle)
    {
        this.collapseStyle = collapseStyle;
        this.Title = title;
        this.ContentView = contentView;
        this.Toolbar = toolbar;
        this.IsCollapseVisible = canCollapse;
        this.CollapseIconSource = this.CollapseIcon();
        this.ExpandIconSource = this.ExpandIcon();
        this.Collapse(collapse: false);
    }

    public static HeaderedContentView CreateContent<TViewModel, TControl, TToolbarViewModel, TToolbarControl>(
        string title, bool canCollapse,
        CollapseStyle collapseStyle = CollapseStyle.Left,
        bool createCollapsed = false)
        where TViewModel : ViewModel<TControl>, new()
        where TControl : Control, IView, new()
        where TToolbarViewModel : ViewModel<TToolbarControl>, new()
        where TToolbarControl : Control, IView, new()
    {
        var baseVm = new TViewModel();
        baseVm.CreateViewAndBind();
        var toolbarVm = new TToolbarViewModel();
        toolbarVm.CreateViewAndBind();
        var toolbarView = toolbarVm.View;
        var headerVm =
            new HeaderedContentViewModel(title, canCollapse, baseVm.View, toolbarView, collapseStyle);
        headerVm.CreateViewAndBind();
        if (createCollapsed)
        {
            headerVm.Collapse(true);
        }

        // Attach the behavior on the toolbar view model
        new DisabledOnModal().Attach(toolbarVm);

        return headerVm.View;
    }

    [RelayCommand]
    public void OnCollapse() => this.Collapse(collapse: true);

    [RelayCommand]
    public void OnExpand() => this.Collapse(collapse: false);

    public void Collapse(bool collapse = true)
    {
        if ((this.collapseStyle == CollapseStyle.Left) || (this.collapseStyle == CollapseStyle.Right))
        {
            this.IsSideCollapsed = collapse;
        }
        else
        {
            this.IsUpdownCollapsed = collapse;
        }

        this.IsExpanded = !collapse;
    }

    private string CollapseIcon()
        => this.collapseStyle switch
        {
            CollapseStyle.Top => "chevron_up",
            CollapseStyle.Bottom => "chevron_down",
            CollapseStyle.Right => "chevron_right",
            _ => "chevron_left",
        };

    private string ExpandIcon()
        => this.collapseStyle switch
        {
            CollapseStyle.Top => "chevron_down",
            CollapseStyle.Bottom => "chevron_up",
            CollapseStyle.Right => "chevron_left",
            _ => "chevron_right",
        };
}
