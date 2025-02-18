namespace Lyt.Quantics.Studio.Controls.Histogram;

public sealed class HistogramBarViewModel : Bindable<HistogramBarView>
{
    public HistogramBarViewModel(double value, string label, double multiplier)
    {
        this.DisablePropertyChangedLogging = true;
        this.Value = string.Format("{0:F2}", 100.0 * value);
        this.Height = 2.4 * 100.0 * value * multiplier;
        char[] text = new char[label.Length + label.Length / 4];
        int j = 0;
        for (int i = 0; i < label.Length; ++i)
        {
            text[j] = label[i];
            ++j;
            if ((i > 0) && (0 == (1+i) % 4))
            {
                text[j] = ' ';
                ++j;
            }
        }

        this.Label = new string(text);
        this.FontSize =
            label.Length <= 6 ?
                16.0 :
                label.Length <= 8 ? 14.0 :
                label.Length <= 12 ? 12.0 : 11.0;
    }

    public string? Value { get => this.Get<string?>(); set => this.Set(value); }

    public string? Label { get => this.Get<string?>(); set => this.Set(value); }

    public double Height { get => this.Get<double>(); set => this.Set(value); }

    public double FontSize { get => this.Get<double>(); set => this.Set(value); }
}
