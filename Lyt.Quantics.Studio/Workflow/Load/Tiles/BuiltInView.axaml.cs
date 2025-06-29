namespace Lyt.Quantics.Studio.Workflow.Load.Tiles;

public partial class BuiltInView : View
{
    public BuiltInView() : base() 
    {
        this.PointerEntered += this.OnPointerEnter;
        this.PointerExited += this.OnPointerLeave;
        this.SetVisible(visible: false);
    }

    ~BuiltInView()
    {
        this.PointerEntered -= this.OnPointerEnter;
        this.PointerExited -= this.OnPointerLeave;
    }

    private void OnPointerEnter(object? sender, PointerEventArgs args)
    {
        if ((sender is BuiltInView view) && (this == view))
        {
            this.SetVisible();
        }
    }

    private void OnPointerLeave(object? sender, PointerEventArgs args)
    {
        if ((sender is BuiltInView view) && (this == view))
        {
            this.SetVisible(visible: false);
        }
    }

    private void SetVisible(bool visible = true)
    {
        this.outerBorder.BorderThickness = new Thickness(visible ? 1.0 : 0.0);
        this.openButton.IsVisible = visible;
    }
}