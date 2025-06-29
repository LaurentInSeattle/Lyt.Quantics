namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class GateEditAngleDialog : View
{
    public GateEditAngleDialog() : base ()
        => this.ValueTextBox.TextChanged += this.OnAnyTextBoxTextChanged;

    ~GateEditAngleDialog()
    {
        this.ValueTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? _, TextChangedEventArgs e)
    {
        if (this.DataContext is GateEditAngleDialogModel gateEditDialogModel)
        {
            gateEditDialogModel.OnEditing();
            e.Handled = true;
        }
    }
}