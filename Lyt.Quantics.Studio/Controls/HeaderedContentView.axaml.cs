namespace Lyt.Quantics.Studio.Controls;

public partial class HeaderedContentView : UserControl
{
    public HeaderedContentView() => this.InitializeComponent();

    public TViewModel ViewModel<TViewModel>() where TViewModel : Bindable
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