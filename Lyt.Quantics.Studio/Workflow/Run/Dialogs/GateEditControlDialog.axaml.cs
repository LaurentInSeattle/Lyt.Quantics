namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class GateEditControlDialog : UserControl
{
    public GateEditControlDialog()
    {
        this.InitializeComponent();
        this.ValueTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
    }

    ~GateEditControlDialog()
    {
        this.ValueTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (this.DataContext is GateEditControlDialogModel gateEditControlDialogModel)
        {
            gateEditControlDialogModel.OnEditing();
            e.Handled = true;
        }
    }
}