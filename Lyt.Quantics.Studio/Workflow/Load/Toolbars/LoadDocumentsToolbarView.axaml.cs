namespace Lyt.Quantics.Studio.Workflow.Load.Toolbars;

public partial class LoadDocumentsToolbarView : UserControl, IView
{
    public LoadDocumentsToolbarView()
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