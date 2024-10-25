﻿namespace Lyt.Quantics.Studio.Controls;

public sealed class HeaderedContentViewModel : Bindable<HeaderedContentView>
{
    public enum CollapseStyle
    {
        Left, 
        Right, 
        Top,
        Bottom,
    }

    private readonly bool canCollapse; 
    private readonly CollapseStyle collapseStyle;

    public HeaderedContentViewModel(
        string title, bool canCollapse, 
        Control contentView, Control? toolbar,
        CollapseStyle collapseStyle)
    {
        base.DisablePropertyChangedLogging = true;
        base.DisableAutomaticBindingsLogging = true;

        this.canCollapse = canCollapse;
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
        where TViewModel : Bindable<TControl>, new()
        where TControl : Control, new()
        where TToolbarViewModel : Bindable<TToolbarControl>, new()
        where TToolbarControl : Control, new()
    {
        var baseVm = new TViewModel();
        baseVm.CreateViewAndBind();
        var toolbarVm = new TToolbarViewModel();
        toolbarVm.CreateViewAndBind();
        var headerVm =
            new HeaderedContentViewModel(title, canCollapse, baseVm.View, toolbarVm.View, collapseStyle);
        headerVm.CreateViewAndBind();
        if (createCollapsed)
        {
            headerVm.Collapse(true);
        }

        return headerVm.View;
    }

    public void Collapse(bool collapse = true)
    {
        if ((this.collapseStyle == CollapseStyle.Left)|| (this.collapseStyle == CollapseStyle.Right))
        {
            this.IsSideCollapsed = collapse;
        }
        else
        {
            this.IsUpdownCollapsed = collapse;
        }

        this.IsExpanded = !collapse;
    }

    private string CollapseIcon ()
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

    private void OnCollapse(object? _) => this.Collapse(collapse: true);

    private void OnExpand(object? _) => this.Collapse(collapse: false);

    public string? Title { get => this.Get<string?>(); set => this.Set(value); }

    public string? CollapseIconSource { get => this.Get<string?>(); set => this.Set(value); }

    public string? ExpandIconSource { get => this.Get<string?>(); set => this.Set(value); }

    public bool IsCollapseVisible { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsSideCollapsed { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsUpdownCollapsed { get => this.Get<bool>(); set => this.Set(value); }

    public bool IsExpanded { get => this.Get<bool>(); set => this.Set(value); }

    public ICommand CollapseCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand ExpandCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public Control? Toolbar { get => this.Get<Control?>()!; set => this.Set(value); }

    public Control ContentView { get => this.Get<Control>()!; set => this.Set(value); }
}
