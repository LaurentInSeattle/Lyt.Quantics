namespace Lyt.QuantumSimulator.UnitTests;

public static class Driver
{
    public static void Run(Func<bool> action, int loops = 10_000, double probability = 0.025)
    {
        double diff = 0.0;
        for (int i = 0; i < loops; i++)
        {
            Qubit.ClearCache();
            bool result = action();
            diff += result ? 1 : -1;
        }

        Assert.IsTrue(Math.Abs(diff) / loops < probability);
    }
}
