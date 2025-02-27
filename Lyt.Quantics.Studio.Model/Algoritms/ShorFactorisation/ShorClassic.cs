namespace Lyt.Quantics.Studio.Model.Algoritms.ShorFactorisation;

public static class ShorClassic
{
    public static void Poke()
    {
        // See: https://en.wikipedia.org/wiki/List_of_prime_numbers
        int[] primes = [29, 67, 89, 101, 239, 631, 1013, 5557, 6043, 7727, 7919];

        for (int k = 0; k < 10; ++ k)
        {
            for (int i = 0; i < primes.Length - 1; ++i)
            {
                int p1 = primes[i];
                for (int j = 1 + i; j < primes.Length; ++j)
                {
                    int p2 = primes[j];
                    var start = DateTime.Now;
                    var tuple = Factorize(p1 * p2);
                    var end = DateTime.Now;
                    Debug.WriteLine("Submitted Factors " + p1 + " - " + p2);
                    Debug.WriteLine("Elapsed: " + (end - start).TotalSeconds.ToString("F3"));

                    int min = Math.Min(tuple.Item1, tuple.Item2);
                    int max = Math.Max(tuple.Item1, tuple.Item2);
                    if ((min != p1) || (max != p2))
                    {
                        if (Debugger.IsAttached) Debugger.Break();
                    }
                }
            }
        } 
    }

    /// Return the greatest comon divisor of nonnegative numbers, not both 0.
    public static int GreatestCommonDivisor(int a, int b)
    {
        if ((a == 0) && (b == 0))
        {
            throw new ArgumentException("Numbers both zero.");
        }

        while (b != 0)
        {
            int r = a % b;
            a = b;
            b = r;
        }

        return a;
    }

    // Calculate k to the power of power 
    public static int IntPower(int k, int power)
    {
        for (int i = 1; i < power; ++i)
        {
            k *= k;
        }

        return k;
    }

    /// <summary> Find mininum value (r) such as a to the power of r mod n is one  </summary>
    public static int FindOrder(int a, int n)
    {
        for (int r = 2; r < 13; r += 2)
        {
            // Calculate a to the power of r by 2
            int determinant = IntPower(a, r);
            if (1 == determinant % n)
            {
                return r;
            }
        }

        return 0;
    }

    // See : https://www.youtube.com/watch?v=tnctwK-9-G4&list=PLl0eQOWl7mnUTNF7KlDJVI3PUgyaXQUHS&index=1
    public static Tuple<int, int> Factorize(int n)
    {
        Tuple<int, int> tuple = new(0, 0);
        Debug.WriteLine("Factorizing " + n);
        bool finished = false;
        bool isLucky = false;
        int loop = 0;
        int order = 0;

        // Using variables names that match the YT video presentation 
        while (!finished)
        {
            ++loop;

            int a = RandomUtility.NextInt(n);
            if ((a == 1) || (a == n))
            {
                throw new ArgumentException("Invalid random number.");
            }

            //if ( ( 0 == a % 2 ) && ( a > 5 ))
            //{
            //    a = a - 1; 
            //}

            // Debug.WriteLine("Random " + a + "   Loop: " + loop);

            int gcd = GreatestCommonDivisor(a, n);
            if (gcd > 1)
            {
                // Super lucky case
                isLucky = true;
                finished = true;
                tuple = new Tuple<int, int>(gcd, n / gcd);
            }
            else
            {
                int r = FindOrder(a, n);
                if ((r > 0) && (0 == r % 2)&& (r<13))
                {
                    int rBy2 = r / 2;

                    // Calculate a to the power of r by 2
                    // OVERFLOW Here !!!
                    // Bigger than N ??? 
                    int determinant = IntPower(a, rBy2);
                    if (determinant < 0)
                    {
                        // Fail, go get a new random A value and loop 
                    }
                    else
                    {
                        int i = determinant - 1;
                        int j = determinant + 1;
                        if ((0 == i % n) || (0 == j % n))
                        {
                            // Fail, go get a new random A value and loop 
                        }
                        else
                        {
                            // Success !
                            order = r;
                            i = i % n;
                            j = j % n;
                            int p1 = GreatestCommonDivisor(i, n);
                            int p2 = GreatestCommonDivisor(j, n);
                            finished = true;
                            tuple = new Tuple<int, int>(p1, p2);
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
}
