namespace Lyt.Quantics.Engine.Tests;

[TestClass]
public sealed class Tests_PreloadedSwaps
{
    [TestMethod]
    public void Test_Serialization()
    {
        var swaps = new NestedDictionary<int, int, int, List<Swap>>
        {
            { 2, 0, 1, [new Swap(0, 1), new Swap(1, 0)] },
            { 3, 0, 1, [new Swap(0, 1), new Swap(1, 0)] },
            { 3, 0, 2, [new Swap(0, 2), new Swap(2, 0)] },
            { 3, 1, 2, [new Swap(1, 2), new Swap(2, 1)] }
        };

        string swapStrings = SerializationUtilities.Serialize(swaps);
        Debug.WriteLine(swapStrings);
        var data = SerializationUtilities.Deserialize<NestedDictionary<int, int, int, List<Swap>>>(swapStrings);
    }

    // [TestMethod]
    public void Test_GenerateSwaps ()
    {
        var swaps = new NestedDictionary<int, int, int, List<Swap>>();
        var stopwatch = new Stopwatch(); 
        stopwatch.Start();

        for ( int qubits = 17; qubits < 18; ++ qubits)
        {
            Debug.WriteLine("Starting: " + qubits);
            var register = new QuRegister(qubits);
            var ketMap = new KetMap(qubits); 

            for (int i = 0; i < qubits ; ++i)
            {
                for (int j = i + 1 ; j < qubits; ++j)
                {
                    var swap = register.GenerateSwaps(ketMap, i, j);
                    if (( i == 0 ) && (j == 1))
                    {
                        Debug.WriteLine("Swaps: " + swap.Count);
                    } 

                    swaps.Add(qubits, i, j, swap); 
                }

                Debug.WriteLine("Stage: " + i);
                Debug.WriteLine("Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));
            }

            Debug.WriteLine("Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));
        }

        Debug.WriteLine("Serializing..." );
        string swapStrings = SerializationUtilities.Serialize(swaps);
        Debug.WriteLine("Swap Strings: " + swapStrings.Length);
        Debug.WriteLine("Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));

        byte[] swapsBinary = CompressionUtilities.Compress(swapStrings);
        Debug.WriteLine("Compressed: Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));

        string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string path = Path.Combine(docs, "test.binary");

        File.WriteAllBytes( path, swapsBinary);
        Debug.WriteLine("Saved: Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));

        byte[] readBinary = File.ReadAllBytes(path);
        string read = CompressionUtilities.DecompressToString(readBinary);
        Debug.WriteLine("Read + Decompressed: Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));
        var loaded = SerializationUtilities.Deserialize<NestedDictionary<int, int, int, List<Swap>>>(read);
        Debug.WriteLine("Deserialisation: Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));
    }
} 