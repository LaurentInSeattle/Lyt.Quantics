namespace Lyt.Quantics.Studio.Controls;

public sealed partial class AmplitudeMinibarViewModel : ViewModel<AmplitudeMinibarView>
{
    [ObservableProperty]
    private double height;

    [ObservableProperty]
    private bool visible;

    public AmplitudeMinibarViewModel(double value, bool visible = true)
    {
        // TODO: Fix the 48.0 Magic !
        this.Height = 48.0 * value;
        this.Visible = visible;
    }
}
