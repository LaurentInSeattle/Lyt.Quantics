namespace Lyt.Quantics.Studio.Model.Algoritms.ShorFactorisation;

public static class ShorClassic
{
    public static void Poke()
    {
        //for (int i = 0; i < 50; ++i)
        //{
        //    Factorize(67 * 89);
        //}
    }

    //static ShorClassic() => primes = GetAllPrimesLessThan(int.MaxValue - 2);

    //public static void Poke()
    //{
    //    // See: https://en.wikipedia.org/wiki/List_of_prime_numbers
    //    // int[] primes = [29, 67, 89, 101, 239, 631, 1013, 5557, 6043, 7727, 7919];

    //    // Delicate primes
    //    // Primes that having any one of their(base 10) digits changed to any other value will always result in a composite number.
    //    long[] primes = 
    //        [
    //            294001, 505447, 584141, 604171, 971767, 
    //            1062599, 1282529, 1524181, 
    //            2017963, 2474431, 2690201, 
    //            3085553, 3326489, 
    //            4393139
    //        ];

    //    for (int k = 0; k < 1; ++ k)
    //    {
    //        for (int i = 0; i < primes.Length - 1; ++i)
    //        {
    //            long p1 = primes[i];
    //            for (int j = 1 + i; j < primes.Length; ++j)
    //            {
    //                long p2 = primes[j];
    //                var start = DateTime.Now;
    //                long n = p1 * p2; 
    //                var tuple = NaiveFactorize(n);
    //                var end = DateTime.Now;
    //                Debug.WriteLine("Submitted Factors " + p1 + " - " + p2);
    //                Debug.WriteLine("Elapsed: " + (end - start).TotalSeconds.ToString("F3"));

    //                long min = Math.Min(tuple.Item1, tuple.Item2);
    //                long max = Math.Max(tuple.Item1, tuple.Item2);
    //                if ((min != p1) || (max != p2))
    //                {
    //                    if (Debugger.IsAttached)
    //                    {
    //                        Debugger.Break();
    //                    }
    //                }
    //            }
    //        }
    //    } 
    //}

    private static readonly List<long> primes = [];

    private static List<long> GetAllPrimesLessThan(int maxPrime)
    {
        var start = DateTime.Now;

        long maxSquareRoot = (long)Math.Sqrt(maxPrime);
        int capacity = (int)Math.Min(maxSquareRoot, int.MaxValue);
        var primes = new List<long>(capacity);
        var eliminated = new FastBitArray(maxPrime + 1);

        for (int i = 2; i <= maxPrime; ++i)
        {
            if (!eliminated[i])
            {
                primes.Add(i);
                if (i <= maxSquareRoot)
                {
                    for (int j = i * i; j <= maxPrime - i; j += i)
                    {
                        eliminated[j] = true;
                    }
                }
            }
        }

        var end = DateTime.Now;
        Debug.WriteLine("GetAllPrimesLessThan: Elapsed: " + (end - start).TotalSeconds.ToString("F3"));

        return primes;
    }

    /// Return the greatest comon divisor of nonnegative numbers, not both 0.
    public static long GreatestCommonDivisor(long a, long b)
    {
        if ((a == 0) && (b == 0))
        {
            throw new ArgumentException("Numbers both zero.");
        }

        while (b != 0)
        {
            long r = a % b;
            a = b;
            b = r;
        }

        return a;
    }

    // Calculate k to the power of power 
    public static long IntPower(long k, int power)
    {
        for (int i = 1; i < power; ++i)
        {
            k *= k;
            if (k < 0)
            {
                // Overflow 
                return 0;
            }
        }

        return k;
    }

    /// <summary> 
    /// Find the EVEN mininum value (r) such as (a) to the power of (r) mod (n) is one  
    /// </summary>
    /// <remarks> Retarded version that almost never returns anything useful. </remarks>
    public static int FindOrder(long a, long n)
    {
        long b = a;
        for (int r = 2; r < 15; ++r)
        {
            long determinant = a * a;
            if (determinant < 0)
            {
                // Overflow 
                return 0;
            }

            a = determinant % n;
            if (1 == a)
            {
                Debug.WriteLine("Order of " + b + " mod " + n + "  : " + r);
                return r;
            }
        }

        return 0;
    }

    // See : https://www.youtube.com/watch?v=tnctwK-9-G4&list=PLl0eQOWl7mnUTNF7KlDJVI3PUgyaXQUHS&index=1
    public static Tuple<long, long> Factorize(long n)
    {
        Tuple<long, long> tuple = new(0, 0);
        Debug.WriteLine("Factorizing " + n);
        bool finished = false;
        bool isLucky = false;
        int loop = 0;
        int order = 0;

        // Using variables names that match the YT video presentation 
        while (!finished)
        {
            ++loop;

            long a = RandomUtility.NextLong(n);
            if ((a == 1) || (a == n))
            {
                throw new ArgumentException("Invalid random number.");
            }

            long gcd = GreatestCommonDivisor(a, n);
            if (gcd == n)
            {
                // Fail, go get a new random A value and loop 
            }
            else if (gcd > 1)
            {
                // Super lucky case
                isLucky = true;
                finished = true;
                tuple = new Tuple<long, long>(gcd, n / gcd);
            }
            else
            {
                int r = FindOrder(a, n);
                if ((r > 0) && (0 == r % 2) && (r < 15))
                {
                    int rBy2 = r / 2;

                    // Calculate a to the power of r by 2
                    long determinant = IntPower(a, rBy2);
                    if (determinant < 0)
                    {
                        // OVERFLOW Here !!! Bigger than N ??? 
                        // Fail, go get a new random A value and loop 
                    }
                    else
                    {
                        long i = determinant - 1;
                        long j = determinant + 1;
                        if ((0 == i % n) || (0 == j % n))
                        {
                            // Fail, go get a new random A value and loop 
                        }
                        else
                        {
                            // Success !
                            order = r;
                            i %= n;
                            j %= n;
                            long p1 = GreatestCommonDivisor(i, n);
                            long p2 = GreatestCommonDivisor(j, n);
                            finished = true;
                            tuple = new Tuple<long, long>(p1, p2);
                        }
                    }
                }
            }
        }

        Debug.WriteLine(
            "Factors " + tuple.Item1 + " - " + tuple.Item2 +
            "   Loops: " + loop + "   Lucky: " + isLucky + "   Order: " + order);
        return tuple;
    }

    public static Tuple<long, long> NaiveFactorize(long n)
    {
        Tuple<long, long> tuple = new(0, 0);
        Debug.WriteLine("Naive Factorizing " + n);
        int squareRoot = (int)Math.Sqrt(n);
        bool finished = false;
        int loop = -1;
        // int maxPrime = Math.Min(squareRoot, int.MaxValue - 2);
        int startIndex = primes.Count - 1;
        while (primes[startIndex] > squareRoot)
        {
            --startIndex;
        }

        while (!finished)
        {
            ++loop;

            if (loop >= primes.Count)
            {
                break;
            }

            long a = primes[startIndex - loop];

            // Debug.WriteLine("Random " + a + "   Loop: " + loop);

            long gcd = GreatestCommonDivisor(a, n);
            if (gcd > 1)
            {
                // Happy case
                finished = true;
                tuple = new Tuple<long, long>(gcd, n / gcd);
            }
            // else: Fail, go get a new prime value and loop             
        }

        if (!finished)
        {
            Debug.WriteLine("Overflow !");
        }
        else
        {
            Debug.WriteLine("Factors " + tuple.Item1 + " - " + tuple.Item2 + "   Loops: " + loop);
        }

        return tuple;
    }
}
