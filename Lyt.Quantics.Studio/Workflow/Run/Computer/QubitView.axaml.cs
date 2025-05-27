namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public partial class QubitView : UserControl, IView
{
    public QubitView()
    {
        this.InitializeComponent();
        this.DataContextChanged += this.OnQubitViewDataContextChanged;
        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
    }

    private void OnQubitViewDataContextChanged(object? sender, EventArgs e) 
    {
        if (this.DataContext is QubitViewModel qubitViewModel)
        {
            qubitViewModel.BindOnDataContextChanged(this);
        }
    }
}