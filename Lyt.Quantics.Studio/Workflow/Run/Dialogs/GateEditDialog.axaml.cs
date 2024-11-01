namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class GateEditDialog : UserControl
{
    public GateEditDialog()
    {
        this.InitializeComponent();
        this.PointerPressed += this.OnPointerPressed;
    }

    private void OnPointerPressed(object? sender, PointerPressedEventArgs pointerPressedEventArgs)
    {
        Debug.WriteLine("Pressed");
        if (this.DataContext is GateEditDialogModel model)
        {
            model.OnDismiss(null); 
        } 
    }
}