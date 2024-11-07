namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class GateEditAngleDialog : UserControl
{
    public GateEditAngleDialog()
    {
        this.InitializeComponent();
        this.ValueTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
    }

    ~GateEditAngleDialog()
    {
        this.ValueTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (this.DataContext is GateEditAngleDialogModel gateEditDialogModel)
        {
            gateEditDialogModel.OnEditing();
            e.Handled = true;
        }
    }
}