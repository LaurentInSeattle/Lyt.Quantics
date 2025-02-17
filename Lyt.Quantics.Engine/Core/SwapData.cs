namespace Lyt.Quantics.Engine.Core;

public sealed record class Swap(int Index1, int Index2);

public sealed class SwapData
{
    public enum LoadState
    {
        Nothing, 
        UpToSixteen,
        Seventeen,
        Eighteen,
    }

    private const string Swap_3_16_FileName = "Swaps_3_16.binary";
    private const string Swap_17_FileName = "Swaps_18.binary";
    private const string Swap_18_0_FileName = "Swaps_18_0.binary";
    private const string Swap_18_1_FileName = "Swaps_18_1.binary";
    private const string Swap_18_2_FileName = "Swaps_18_2.binary";

    public static LoadState State { get; private set; } = LoadState.Nothing; 

    private static NestedDictionary<int, int, int, List<Swap>> PreloadedSwaps { get; set; } = [];

    static SwapData()
    {
        if( PreloadedSwaps.Count == 0)
        {
            Load();
            State = LoadState.UpToSixteen; 
        }
    }

    public static void Load(string filename = Swap_3_16_FileName)
    {
        try
        {
            byte[] swapBinaryData = SerializationUtilities.LoadEmbeddedBinaryResource(filename, out string? _);
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

    public static void OnQuBitCountChanged (int qubitCount)
    {
        if ( ( qubitCount < 0 ) || ( qubitCount > QuRegister.MaxQubits))
        {
            throw new Exception("Invalid QuBit Count"); 
        }

        if ( qubitCount <= 16)
        {
            if (State == LoadState.UpToSixteen)
            {
                return;
            }

            Load(Swap_3_16_FileName);
            State = LoadState.UpToSixteen;
        }
        else if (qubitCount == 17)
        {
            if (State == LoadState.Seventeen)
            {
                return;
            }

            Load(Swap_17_FileName);
            State = LoadState.Seventeen;
        }
        else if (qubitCount == 18)
        {
            if (State == LoadState.Eighteen)
            {
                return;
            }

            Load(Swap_17_FileName);
            State = LoadState.Seventeen;
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