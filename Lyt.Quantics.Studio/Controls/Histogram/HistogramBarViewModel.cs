namespace Lyt.Quantics.Studio.Controls.Histogram;

public sealed class HistogramBarViewModel : Bindable<HistogramBarView>
{
    public HistogramBarViewModel(double value, string label, double multiplier)
    {
        this.DisablePropertyChangedLogging = true;
        this.Value = string.Format ( "{0:F1}", 100.0 * value) ; 
        this.Label = label;
        this.Height = 2 * 100.0 * value * multiplier ;
    }

    public string? Value { get => this.Get<string?>(); set => this.Set(value); }

    public string? Label { get => this.Get<string?>(); set => this.Set(value); }

    public double Height { get => this.Get<double>(); set => this.Set(value); }
}
