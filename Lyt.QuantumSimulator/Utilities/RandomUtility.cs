﻿namespace Lyt.QuantumSimulator.Utilities;

public static class RandomUtility
{
    public static double NextDouble()
    {
        return rd.NextDouble();
    }

    // USE : https://learn.microsoft.com/en-us/dotnet/api/system.security.cryptography.randomnumbergenerator?view=net-8.0 
    /*
     static double NextDouble()
{
ulong nextULong = BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(sizeof(ulong)));

return (nextULong >> 11) * (1.0 / (1ul << 53));

byte[] bytes = RandomNumberGenerator.GetBytes(8);
// bit-shift 11 and 53 based on double's mantissa bits
ulong ul = BitConverter.ToUInt64(bytes, 0) / (1 << 11);
double d = (double)ul / (double)(1UL << 53);

}
     */
    private static Random rd = new Random(
        Guid.NewGuid().GetHashCode()
    );
}
