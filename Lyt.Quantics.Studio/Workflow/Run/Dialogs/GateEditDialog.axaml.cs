namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class GateEditDialog : UserControl
{
    public GateEditDialog()
    {
        this.InitializeComponent();
        this.ValueTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
    }

    ~GateEditDialog()
    {
        this.ValueTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (this.DataContext is GateEditDialogModel gateEditDialogModel)
        {
            gateEditDialogModel.OnEditing();
            e.Handled = true;
        }
    }
}