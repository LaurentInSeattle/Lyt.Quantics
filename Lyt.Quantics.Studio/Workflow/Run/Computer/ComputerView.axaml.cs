namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public partial class ComputerView : View
{
    public ComputerView() :base ()
    {
        this.Tapped += (s, e) =>
        {
            if ( this.DataContext is ComputerViewModel computerViewModel)
            {
                bool handled = false; 
                var position = e.GetPosition(this);
                double ratio = position.X / this.Bounds.Width; 
                if ( ( ratio < 0.1 ) || ( ratio > 0.9 ))
                {
                    handled = computerViewModel.OnClicked(isLeft : ratio < 0.1 );
                }

                e.Handled = handled; 
            }
        };
    }

    protected override void OnDataContextChanged(object? sender, EventArgs e)
    {
        base.OnDataContextChanged(sender, e);
        new DragOverAble(StageView.HideDropTarget).Attach(this);
        new DropAble(StageView.HideDropTarget).Attach(this);
    }
}