﻿namespace Lyt.Quantics.Engine.Utilities;

public static class RandomUtility
{
    private static readonly Random random = new(Guid.NewGuid().GetHashCode());

    public static double NextDouble() => random.NextDouble();

    public static int NextInt(int n)
    {
        double x = NextDouble();
        int r = (int)((Math.Floor(n * x) - 1.0));
        return 1 + r;
    }

    public static long NextLong(long n)
    {
    Begin: 
        double x = NextDouble();
        long r = (long)((Math.Floor(n * x) - 1.0));
        long a =  1 + r;

        if ((a == 1) || (a == n))
        {
            goto Begin;
        }

        return a; 
    }

    // TODO: Improve 

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
}
