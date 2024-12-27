namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

public partial class LoadBuiltInToolbarView : UserControl
{
    public LoadBuiltInToolbarView()
    {
        this.InitializeComponent();
        this.FilterTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
    }

    ~LoadBuiltInToolbarView()
    {
        this.FilterTextBox.TextChanged -= this.OnAnyTextBoxTextChanged;
    }

    private void OnAnyTextBoxTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (this.DataContext is LoadBuiltInToolbarViewModel loadBuiltInToolbarViewModel)
        {
            loadBuiltInToolbarViewModel.OnEditing();
            e.Handled = true;
        }
    }
}