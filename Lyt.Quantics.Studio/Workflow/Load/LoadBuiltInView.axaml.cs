namespace Lyt.Quantics.Studio.Workflow.Load;

public partial class LoadBuiltInView : UserControl, IView
{
    public LoadBuiltInView()
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