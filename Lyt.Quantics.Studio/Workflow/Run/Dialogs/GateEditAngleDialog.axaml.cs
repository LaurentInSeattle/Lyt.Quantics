namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class GateEditAngleDialog : UserControl, IView
{
    public GateEditAngleDialog()
    {
        this.InitializeComponent();
        this.ValueTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
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