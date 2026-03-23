namespace Lyt.Quantics.Studio.Controls;

public sealed partial class AmplitudeMinibarViewModel : ViewModel<AmplitudeMinibarView>
{
    [ObservableProperty]
    public partial double Height { get; set; }

    [ObservableProperty]
    public partial bool Visible { get; set; }

    public AmplitudeMinibarViewModel(double value, bool visible = true)
    {
        // TODO: Fix the 48.0 Magic !
        this.Height = 48.0 * value;
        this.Visible = visible;
    }
}
