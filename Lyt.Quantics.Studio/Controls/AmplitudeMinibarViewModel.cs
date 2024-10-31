namespace Lyt.Quantics.Studio.Controls;

public sealed class AmplitudeMinibarViewModel : Bindable<AmplitudeMinibarView>
{
    // TODO: Fix the 48.0 Magic !

    public AmplitudeMinibarViewModel(double value, bool visible = true)
    {
        this.DisablePropertyChangedLogging = true;
        this.Height = 48.0 * value;
        this.Visible = visible;
    }

    public double Height { get => this.Get<double>(); set => this.Set(value); }

    public bool Visible { get => this.Get<bool>(); set => this.Set(value); }
}
