namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

public partial class LoadBuiltInToolbarView : UserControl, IView
{
    public LoadBuiltInToolbarView()
    {
        this.InitializeComponent();
        this.FilterTextBox.TextChanged += this.OnAnyTextBoxTextChanged;
        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
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