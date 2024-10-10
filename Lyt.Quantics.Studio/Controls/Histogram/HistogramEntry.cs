namespace Lyt.Quantics.Studio.Controls.Histogram
{
    public sealed class HistogramEntry (double value, string label) 
    {
        public double Value { get; private set; } = value;

        public string Label { get; private set; } = label; 
    }
}
