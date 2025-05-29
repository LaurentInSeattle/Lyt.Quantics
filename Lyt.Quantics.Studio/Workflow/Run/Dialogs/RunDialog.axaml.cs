namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class RunDialog : UserControl, IView
{
    public RunDialog()
    {
        this.InitializeComponent();
        this.Loaded += (s, e) =>
        {
            if (this.DataContext is not null && this.DataContext is ViewModel viewModel)
            {
                viewModel.OnViewLoaded();
            }
        };
    }
}