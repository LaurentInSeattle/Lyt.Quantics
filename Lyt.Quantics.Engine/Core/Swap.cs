namespace Lyt.Quantics.Engine.Core;

public sealed record class Swap(int Index1, int Index2);

public sealed class SwapData
{
    private static NestedDictionary<int, int, int, List<Swap>> swaps { get; set; } = new();

    public NestedDictionary<int, int, int, List<Swap>> Swaps { get; set; } = new();

    static SwapData ()
    {
    }
} 