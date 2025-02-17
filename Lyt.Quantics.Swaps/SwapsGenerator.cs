namespace Lyt.Quantics.Swaps;

public sealed class SwapsGenerator(int qubitCount)
{
    private readonly List<Tuple<int, int>> swaps = new(1024);
    private readonly HashSet<ulong> processedSwaps = new(1024);

    public int QuBitCount { get; private set; } = qubitCount;

    public int StateCount { get; private set; } = MathUtilities.TwoPower(qubitCount);

    public KetMap KetMap { get; private set; } = new KetMap(qubitCount); 

    public List<Swap> GenerateSwaps(int i, int j)
    {
        KetMap reducedKetMap = this.KetMap.Reduce(i, j);

        // This is looping over the entire state vector and truly is where there's a need to go as fast as possible 
        void ProcessStateVector(int from, int to, bool withLocks)
        {
            for (int k1 = from; k1 < to; ++k1)
            {
                // for the state at k index: 
                // if bit #i is not equal to bit #j, we need to swap values 
                // The condition is equivalent to: 
                // if bit #i is set an bit #j is not set 
                // OR 
                // if bit #j is set an bit #i is not set 
                // 
                // WAS: bool areDifferent = ketMap.Get(k1, i) ^ ketMap.Get(k1, j);
                bool[] fastMapAtK1 = this.KetMap.FastMapBits[k1];
                bool areDifferent = fastMapAtK1[i] ^ fastMapAtK1[j];
                if (areDifferent)
                {
                    // This index k1 needs to be swapped with another one, k2, so... Find k2 
                    int k2 = this.KetMap.SwapMatch(reducedKetMap, k1, i, j);
                    if (withLocks)
                    {
                        lock (this.swaps)
                        {
                            this.swaps.Add(new(k1, k2));
                        }
                    }
                    else
                    {
                        this.swaps.Add(new(k1, k2));
                    }
                }
            }
        }

        this.swaps.Clear();
        this.processedSwaps.Clear();

        if (this.QuBitCount >= QuRegister.ThreadedRunAtQubits)
        {
            // Speed up the processing of the state vector using threads (aka tasks) 
            // 1 : Setup
            int count = 4; // Consider: Environment.ProcessorCount;  
            int all = this.StateCount;
            int half = all / 2;
            int quart = half / 2;
            int[] indices = [0, quart, half, half + quart, all];
            var tasks = new Task[count];
            for (int taskIndex = 0; taskIndex < count; ++taskIndex)
            {
                int from = indices[taskIndex];
                int to = indices[1 + taskIndex];
                var task = new Task(() => ProcessStateVector(from, to, withLocks: true));
                tasks[taskIndex] = task;
            }

            // 2 : Start all tasks
            for (int taskIndex = 0; taskIndex < count; ++taskIndex)
            {
                tasks[taskIndex].Start();
            }

            // 3 : Wait for completion 
            Task.WaitAll(tasks);
        }
        else
        {
            ProcessStateVector(from: 0, to: this.StateCount, withLocks: false);
        }

        List<Swap> generatedSwaps = new(1024);
        foreach (var swap in this.swaps)
        {
            int i1 = swap.Item1;
            int i2 = swap.Item2;
            ulong swapKey = (uint)(i2 << 32) + (uint)i1;
            if (this.processedSwaps.Contains(swapKey))
            {
                continue;
            }

            swapKey = (uint)(i1 << 32) + (uint)i2;
            this.processedSwaps.Add(swapKey);

#if VERBOSE
            Debug.WriteLine(i1 + " <-> " + i2);
#endif // VERBOSE            

            generatedSwaps.Add(new Swap(i1, i2));
        }

        return generatedSwaps;
    }
}
