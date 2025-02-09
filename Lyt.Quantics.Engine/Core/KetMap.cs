// #define VERBOSE 

namespace Lyt.Quantics.Engine.Core;

/*
 
Ket Map Example 

Qubits 4 Swap 0 - 1

0000    <0.222901; 0.974841>    <0.222901; 0.974841>  
0001    <-0.955037; 0.296486>   <-0.955037; 0.296486>  
0010    <-0.921024; -0.389505>   <-0.921024; -0.389505>  
0011    <0.31824; -0.94801>      <0.31824; -0.94801>  

0100    <0.948218; 0.317621>  <-0.939807; 0.341706>      *                   
0101    <-0.244493; 0.969651>  <-0.412279; -0.911058>    * 
0110    <-0.81231; 0.583226>  <0.272654; -0.962112>      *                
0111    <-0.643388; -0.76554>     <0.980081; 0.1986>     *                   

1000    <-0.939807; 0.341706>   <0.948218; 0.317621>     *                  
1001    <-0.412279; -0.911058>  <-0.244493; 0.969651>    *                  
1010    <0.272654; -0.962112>   <-0.81231; 0.583226>     *                  
1011    <0.980081; 0.1986>  <-0.643388; -0.76554>        *                  

1100    <-0.19796; 0.98021>   <-0.19796; 0.98021>      
1101    <-0.992438; -0.122744>  <-0.992438; -0.122744>
1110    <-0.679175; -0.733977>  <-0.679175; -0.733977>
1111    <0.680128; -0.733093>  <0.680128; -0.733093>

 */

/// <summary> 
/// Modelises a list of of lists of booleans providing the ket values for each index in a state vector. 
/// In the example above, this is the first column, where each zero is false, and one is true. 
/// </summary>
public sealed class KetMap
{
    // Made public for performance reason 
    public bool[][] FastMapBits;
    public int [] FastMapValues;

    private readonly int qubitCount;
    private readonly int length;
    private readonly List<List<bool>> map;

    public KetMap(int qubitCount)
    {
        if ((qubitCount < 0) || (qubitCount > QuRegister.MaxQubits))
        {
            throw new ArgumentException("Invalid ket index: ket1");
        }

        this.qubitCount = qubitCount;
        this.length = MathUtilities.TwoPower(qubitCount);
        this.map = new List<List<bool>>(length);

        List<bool> Ket(int index)
        {
            var list = new List<bool>(qubitCount);
            for (int i = 0; i < qubitCount; i++)
            {
                list.Add(MathUtilities.IsBitSet(index, i));
            }

            list.Reverse();
            return list;
        }

        for (int i = 0; i < length; i++)
        {
            this.map.Add(Ket(i));
        }

        this.FastMapBits = new bool[length][];
        for (int i = 0; i < length; i++)
        {
            this.FastMapBits[i] = [.. this.map[i]]; 
        }

        this.FastMapValues = new int[length];
        for (int i = 0; i < length; i++)
        {
            int sum = 0;
            int bitValue = 1;
            bool[] bools = this.FastMapBits[i];
            int boolsLength = bools.Length;
            for (int j = 0; j < boolsLength; ++j)
            {
                if (bools[boolsLength - 1 - j])
                {
                    sum += bitValue; 
                }

                bitValue *= 2; 
            } 

            this.FastMapValues[i] = sum; 
        }

        DumpMap(this.map);
        DumpFastMap(this.FastMapBits, this.FastMapValues);
    }

    /// <summary> Create a new Ket Map by ignoring the values for the two provided qubits indices. </summary>
    public KetMap Reduce(int ket1, int ket2)
    {
        if ((ket1 < 0) || (ket1 >= qubitCount))
        {
            throw new ArgumentException("Invalid ket index: ket1");
        }

        if ((ket2 < 0) || (ket2 >= qubitCount))
        {
            throw new ArgumentException("Invalid ket index: ket2");
        }

        if (ket1 == ket2)
        {
            throw new ArgumentException("Invalid ket indices");
        }

        var min = Math.Min(ket1, ket2);
        var max = Math.Max(ket1, ket2);
        KetMap reducedKetMap = this.DeepClone();
        var reducedMap = reducedKetMap.map;
        DumpMap(reducedMap);

        foreach (var list in reducedMap)
        {
            // The order of the Remove's is important to keep the list properly ordered
            list.RemoveAt(max);
            list.RemoveAt(min);
        }

        DumpMap(reducedMap);

        reducedKetMap.FastMapBits = new bool[length][];
        for (int i = 0; i < length; i++)
        {
            reducedKetMap.FastMapBits[i] = [.. reducedKetMap.map[i]];
        }

        reducedKetMap.FastMapValues = new int[length];
        for (int i = 0; i < length; i++)
        {
            int sum = 0;
            int bitValue = 1;
            bool[] bools = reducedKetMap.FastMapBits[i];
            int boolsLength = bools.Length;
            for (int j = 0; j < boolsLength; ++j)
            {
                if (bools[boolsLength - 1 - j])
                {
                    sum += bitValue;
                }

                bitValue *= 2;
            }

            reducedKetMap.FastMapValues[i] = sum;
        }

        return reducedKetMap;
    }

    public KetMap DeepClone() => new(this.qubitCount);

    /// <summary> Returns the matching index in the state vector when swaping quBits </summary>
    public int SwapMatch(KetMap reducedKetMap, int index, int ket1, int ket2)
    {
#if DEBUG
        if ((index < 0) || (index >= this.map.Count))
        {
            throw new ArgumentException("Invalid index");
        }

        if ((ket1 < 0) || (ket1 >= qubitCount))
        {
            throw new ArgumentException("Invalid ket index: ket1");
        }

        if ((ket2 < 0) || (ket2 >= qubitCount))
        {
            throw new ArgumentException("Invalid ket index: ket2");
        }

        if (ket1 == ket2)
        {
            throw new ArgumentException("Invalid ket indices");
        }

#endif // DEBUG
        var reducedFastMapBits = reducedKetMap.FastMapBits;
        int targetValue = reducedKetMap.FastMapValues[index];
        for (int match = 0; match < reducedFastMapBits.Length; ++match)
        {
            if (index == match)
            {
                continue;
            }

            bool[] bitsAtMatch = this.FastMapBits[match]; 
            if (bitsAtMatch[ket1] == bitsAtMatch[ket2])
            {
                continue;
            }

            if (reducedKetMap.FastMapValues[match] == targetValue)
            {
                return match; 
            } 
        }

        throw new Exception("Ket Match not found");
    }

    [Conditional("DEBUG")]
    private static void DumpMap(List<List<bool>> map)
    {
#if VERBOSE

        Debug.WriteLine("Ket Map:");
        Debug.Indent();
        for (int i = 0; i < map.Count; ++i)
        {
            var bits = map[i];
            foreach (var bit in bits)
            {
                Debug.Write(bit ? "1" : "0");
            }

            Debug.WriteLine("");
        }

        Debug.Unindent();
        Debug.WriteLine("");
#endif // VERBOSE 
    }

    [Conditional("DEBUG")]
    private static void DumpFastMap(bool[][] map, int[] ints)
    {
#if VERBOSE
        Debug.WriteLine("Fast Ket Map:");
        Debug.Indent();
        for (int i = 0; i < map.Length; ++i)
        {
            var bits = map[i];
            foreach (var bit in bits)
            {
                Debug.Write(bit ? "1" : "0");
            }

            Debug.Write(" ");
            Debug.Write(ints[i]);
            Debug.WriteLine("");
        }

        Debug.Unindent();
        Debug.WriteLine("");
#endif // VERBOSE 
    }
}
