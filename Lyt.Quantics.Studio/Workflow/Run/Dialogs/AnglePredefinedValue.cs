namespace Lyt.Quantics.Studio.Workflow.Run.Dialogs;

public sealed record class AnglePredefinedValue(int PiDivisor, bool IsPositive)
{
    public double Value => (this.IsPositive ? 1.0 : -1.0) * Math.PI / PiDivisor;

    public string Caption
        => this.PiDivisor == 1 ?
            string.Format("{0}π", (this.IsPositive ? "+" : "-")) :
            string.Format("{0}π / {1}", (this.IsPositive ? "+" : "-"), this.PiDivisor);
}
