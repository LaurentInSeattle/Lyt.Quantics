namespace Lyt.Quantics.Studio.Model.Algoritms.ShorFactorisation;

public sealed class FastBitArray(long size)
{
    // define a byte buffer sized to hold 8 bools per byte.
    // The spare +1 is to avoid dealing with rounding.
    private readonly byte[] buffer = new byte[size / 8 + 1];

    public bool this[long addr]
    {
        get
        {

            byte mask = (byte)(1 << (int)(addr & 7));
            byte val = this.buffer[(int)(addr >> 3)];
            bool bit = (val & mask) == mask;
            return bit;
        }

        set
        {
            byte mask = (byte)((value ? 1 : 0) << (int)(addr & 7));
            int offs = (int)addr >> 3;
            this.buffer[offs] = (byte)(this.buffer[offs] | mask);
        }
    }
}
