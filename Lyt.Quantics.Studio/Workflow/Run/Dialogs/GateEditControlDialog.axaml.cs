namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class GateEditControlDialog : UserControl, IView
{
    public GateEditControlDialog()
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