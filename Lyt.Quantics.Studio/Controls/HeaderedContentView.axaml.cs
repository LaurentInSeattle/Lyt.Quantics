namespace Lyt.Quantics.Studio.Controls;

public partial class HeaderedContentView : UserControl, IView
{
    public HeaderedContentView() => this.InitializeComponent();

    public TViewModel ViewModel<TViewModel>() where TViewModel : ViewModel
    {
        if ((this.DataContext is HeaderedContentViewModel headeredContentViewModel) &&
            (headeredContentViewModel.ContentView is not null) &&
            (headeredContentViewModel.ContentView.DataContext is TViewModel viewModel))
        {
            return viewModel; 
        }
        else
        {
            throw new Exception("Failed to create and bind ComputerViewModel");
        }
    }
}