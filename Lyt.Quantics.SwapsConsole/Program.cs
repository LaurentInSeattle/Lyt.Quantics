using Lyt.Quantics.Engine.Core;
using Lyt.Quantics.Engine.Utilities;
using Lyt.Quantics.Swaps;
using System.Diagnostics;

namespace Lyt.Quantics.SwapsConsole;

internal class Program
{
    static void Main(string[] _)
    {
        Console.WriteLine("Swap Files Generator");
        Console.WriteLine("");
        int start = 0;
        bool valid = false;
        while (!valid)
        {
            Console.WriteLine("Enter start qubit: ");
            string? inputStart = Console.ReadLine();
            if (int.TryParse(inputStart, out start))
            {
                if ((start > 2) && (start < QuRegister.MaxQubits))
                {
                    valid = true;
                }
            }
        }

        int end = 0;
        valid = false;
        while (!valid)
        {
            Console.WriteLine("Enter end limit qubit: ");
            string? inputEnd = Console.ReadLine();
            if (int.TryParse(inputEnd, out end))
            {
                if ((end > start) && (start <= QuRegister.MaxQubits))
                {
                    valid = true;
                }
            }
        }

        Console.WriteLine("Press enter to start... ");
        Console.ReadLine();

        // TODO: Enter slice for qubits above 16

        Run(start, end);

        Console.WriteLine("Press enter to close this window... ");
        Console.ReadLine();
    }

    static void Run(int startQuBit, int endQuBit)
    {
        var swaps = new NestedDictionary<int, int, int, List<Swap>>();
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        for (int qubits = startQuBit; qubits < endQuBit; ++qubits)
        {
            Console.WriteLine("Starting: " + qubits);
            var generator = new SwapsGenerator(qubits);

            for (int i = 0; i < 3; ++i)
            {
                Console.WriteLine("Stage: " + i);
                for (int j = i + 1; j < qubits; ++j)
                {
                    Console.WriteLine("Sub Stage: " + j);
                    var swap = generator.GenerateSwaps(i, j);
                    if ((i == 0) && (j == 1))
                    {
                        Console.WriteLine("Swaps: " + swap.Count);
                    }

                    swaps.Add(qubits, i, j, swap);
                }

                Console.WriteLine("Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));
            }

            Console.WriteLine("Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));
        }

        Console.WriteLine("Serializing...");
        string swapStrings = SerializationUtilities.Serialize(swaps);
        Console.WriteLine("Swap Strings: " + swapStrings.Length);
        Console.WriteLine("Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));

        byte[] swapsBinary = CompressionUtilities.Compress(swapStrings);
        Console.WriteLine("Compressed: Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));

        string docs = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        string path = Path.Combine(docs, "test.binary");

        File.WriteAllBytes(path, swapsBinary);
        Console.WriteLine("Saved: Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));

        // Read back to verify 
        byte[] readBinary = File.ReadAllBytes(path);
        string read = CompressionUtilities.DecompressToString(readBinary);
        Console.WriteLine("Read + Decompressed: Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));
        var loaded = SerializationUtilities.Deserialize<NestedDictionary<int, int, int, List<Swap>>>(read);
        Console.WriteLine("Deserialisation: Elapsed: " + stopwatch.Elapsed.TotalSeconds.ToString("F1"));
        Console.WriteLine(loaded.GetType());
    }

}
