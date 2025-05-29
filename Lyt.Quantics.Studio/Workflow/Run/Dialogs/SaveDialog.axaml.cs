namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class SaveDialog : UserControl, IView
{
    public SaveDialog()
    {
        this.InitializeComponent();
        this.NameTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
        this.DescriptionTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
    }

    ~SaveDialog()
    {
        this.NameTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
        this.DescriptionTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (this.DataContext is SaveDialogModel saveDialogModel)
        {
            saveDialogModel.OnEditing();
            e.Handled = true;
        }
    }
}