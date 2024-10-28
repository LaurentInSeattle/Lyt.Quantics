namespace Lyt.Quantics.Studio.Controls.Histogram;

public sealed class HistogramBarViewModel : Bindable<HistogramBarView>
{
    public HistogramBarViewModel(double value, string label, double multiplier)
    {
        this.DisablePropertyChangedLogging = true;
        this.Value = string.Format("{0:F1}", 100.0 * value);
        this.Height = 2 * 100.0 * value * multiplier;
        if (label.Length > 4)
        {
            string rightPart = label.Substring(label.Length - 4, 4);
            string leftPart = label[..^4];
            label = string.Concat(leftPart, " ", rightPart);
        }

        this.Label = label;
        this.FontSize =
            label.Length <= 5 ?
                14.0 :
                label.Length <= 6 ? 12.0 : 10.0;
    }

    public string? Value { get => this.Get<string?>(); set => this.Set(value); }

    public string? Label { get => this.Get<string?>(); set => this.Set(value); }

    public double Height { get => this.Get<double>(); set => this.Set(value); }

    public double FontSize { get => this.Get<double>(); set => this.Set(value); }
}
