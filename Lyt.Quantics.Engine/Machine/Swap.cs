namespace Lyt.Quantics.Engine.Machine;

public sealed record class Swap(int First = -1, int Second = -1)
{
    public bool NoSwap => this.First == this.Second;
}
