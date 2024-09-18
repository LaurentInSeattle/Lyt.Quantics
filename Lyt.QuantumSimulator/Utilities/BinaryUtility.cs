namespace Lyt.QuantumSimulator.Utilities;

public static class BinaryUtility
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsBitSet(int x, int pos) => (x & (1 << pos)) != 0;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetBit(int x, int pos) => x | (1 << pos);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ClearBit(int x, int pos) => x & (~(1 << pos));

    // public static int SetBitValue(int x, int pos, bool v) => (x & (~(1 << pos))) | ((v ? 1 : 0) << pos);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int SetBitValue(int x, int pos, bool v) => v ? (x | (1 << pos)) : (x & (~(1 << pos))); 
} 