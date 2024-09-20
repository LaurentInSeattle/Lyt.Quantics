namespace Lyt.QuantumSimulator.UnitTests;

public static class Driver
{
    public static void RunUnary(
        Func<int> action, int loops = 10_000, double expectedProbability = 0.5, double tolerance = 0.015)
    {
        int ones = 0;
        int zeroes = 0;
        for (int i = 0; i < loops; i++)
        {
            int measure = action();
            if (measure == 0)
            {
                ones++;
            }
            else
            {
                zeroes++;
            } 
        }

        double result = ones / (double)loops; 
        Assert.IsTrue( Math.Abs(result - expectedProbability) < tolerance);
    }
}
