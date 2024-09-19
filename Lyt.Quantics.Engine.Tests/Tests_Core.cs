namespace Lyt.Quantics.Engine.Tests;

[TestClass]
public sealed class Tests_Core
{
    #region Fixtures 

    [AssemblyInitialize]
    public static void AssemblyInit(TestContext context)
    {
        // This method is called once for the test assembly, before any tests are run.
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup()
    {
        // This method is called once for the test assembly, after all tests are run.
    }

    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        // This method is called once for the test class, before any tests of the class are run.
    }

    [ClassCleanup]
    public static void ClassCleanup()
    {
        // This method is called once for the test class, after all tests of the class are run.
    }

    [TestInitialize]
    public void TestInit()
    {
        // This method is called before each test method.
    }

    [TestCleanup]
    public void TestCleanup()
    {
        // This method is called after each test method.
    }

    #endregion Fixtures 

    [TestMethod]
    public void Test_Entangle()
    {
        var q1 = new QuBit();
        var q1M = q1.Measure();
        var q2 = new QuBit(1);
        var q2M = q2.Measure();
        var qr = new QuRegister(q1, q2);
        q1M = q1.Measure();
        q1M = q2.Measure();
    }
}
