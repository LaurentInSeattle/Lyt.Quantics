namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public partial class GateEditQubitsDialog : UserControl, IView
{
    public GateEditQubitsDialog()
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