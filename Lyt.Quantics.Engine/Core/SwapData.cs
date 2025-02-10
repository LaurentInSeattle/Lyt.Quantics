namespace Lyt.Quantics.Engine.Core;

public sealed record class Swap(int Index1, int Index2);

public sealed class SwapData
{
    private const string SwapFileName = "Swaps_3_16.binary";

    private static NestedDictionary<int, int, int, List<Swap>> PreloadedSwaps { get; set; } = [];

    static SwapData()
    {
        if( PreloadedSwaps.Count == 0)
        {
            Load(); 
        }
    }

    public static void Load()
    {
        try
        {
            byte[] swapBinaryData = SerializationUtilities.LoadEmbeddedBinaryResource(SwapFileName, out string? _);
            string read = CompressionUtilities.DecompressToString(swapBinaryData);
            var loaded = SerializationUtilities.Deserialize<NestedDictionary<int, int, int, List<Swap>>>(read);
            SwapData.PreloadedSwaps = loaded;
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Failed to load swap data; \n" + ex);
            throw;
        }
    }

    // For unit tests only 
    public static void Poke() { /* will trigger the static CTor */ }

    public static List<Swap> Swaps(int qubit, int i, int j)
    {
        if ((qubit < 0) || (qubit > 16))
        {
            throw new ArgumentException("Invalid qubit index");
        }

        if ((i < 0) || (i >= qubit))
        {
            throw new ArgumentException("Invalid ket index: i");
        }

        if ((j < 0) || (j >= qubit))
        {
            throw new ArgumentException("Invalid ket index: j");
        }

        if (i == j)
        {
            throw new ArgumentException("Invalid indices, cannot be equal.");
        }

        int min = Math.Min(i, j);
        int max = Math.Max(i, j);
        if (SwapData.PreloadedSwaps.TryGetValue(qubit, min, max, out List<Swap>? value))
        {
            if (value is not null)
            {
                return value;
            }
        }

        Debug.WriteLine("Failed to load swap data.");
        throw new Exception("Failed to load swap data.");
    }
}