namespace Lyt.Quantics.Studio.Controls.Histogram;

public sealed class HistogramViewModel : Bindable<HistogramView>
{
    public HistogramViewModel()
    {
        this.DisablePropertyChangedLogging = true;
        //    this.entries =
        //    [
        //        new HistogramEntry(0.20, "000"),
        //        new HistogramEntry(0.15, "001"),
        //        new HistogramEntry(0.05, "010"),
        //        new HistogramEntry(0.10, "011"),
        //        new HistogramEntry(0.20, "100"),
        //        new HistogramEntry(0.15, "101"),
        //        new HistogramEntry(0.05, "110"),
        //        new HistogramEntry(0.10, "111"),
        //    ];

        //    this.entries =
        //    [
        //        new HistogramEntry(0.55, "00"),
        //        new HistogramEntry(0.15, "01"),
        //        new HistogramEntry(0.05, "10"),
        //        new HistogramEntry(0.25, "11"),
        //    ];
    }

    //protected override void OnViewLoaded()
    //{
    //    this.Update(this.entries);
    //}

    public void Update(List<HistogramEntry> entries)
    {
        double max = (from entry in entries select entry.Value).Max();
        double multiplier = 1.0; 
        if (max > 0.50)
        {
            this.Value100 = "100 %";
            this.Value75 = " 75 %";
            this.Value50 = " 50 %";
            this.Value25 = " 25 %";
            this.Value0 = "  0 %";
        }
        else if (max > 0.25)
        {
            multiplier = 2.0;
            this.Value100 = " 50.0 %";
            this.Value75 = "37.5 %";
            this.Value50 = " 25.0 %";
            this.Value25 = "12.5 %";
            this.Value0 = "  0.0 %";
        }
        else
        {
            multiplier = 4.0;
            this.Value100 = "25.00 %";
            this.Value75 = "18.75 %";
            this.Value50 = "12.50 %";
            this.Value25 = " 6.25 %";
            this.Value0 = " 0.00 %";
        }

        var list = new List<HistogramBarViewModel>(entries.Count);
        foreach (var entry in entries)
        {
            var bar = new HistogramBarViewModel(entry.Value, entry.Label, multiplier);
            list.Add(bar);
        }

        this.HistogramBars = list;
    }

    public string? Value0 { get => this.Get<string?>(); set => this.Set(value); }

    public string? Value25 { get => this.Get<string?>(); set => this.Set(value); }

    public string? Value50 { get => this.Get<string?>(); set => this.Set(value); }

    public string? Value75 { get => this.Get<string?>(); set => this.Set(value); }

    public string? Value100 { get => this.Get<string?>(); set => this.Set(value); }

    public List<HistogramBarViewModel>? HistogramBars { get => this.Get<List<HistogramBarViewModel>?>(); set => this.Set(value); }
}