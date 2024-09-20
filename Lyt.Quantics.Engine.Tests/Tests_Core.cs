using Lyt.QuantumSimulator.UnitTests;

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
    public void Test_Basics()
    {
        QuBit.TestVectors();
        var q1 = new QuBit(BasisState.Zero);
        var q1M = q1.Measure();
        var q2 = new QuBit(BasisState.One);
        var q2M = q2.Measure();
        var qr = new QuRegister(q1, q2);
        q1M = q1.Measure();
        q1M = q2.Measure();
    }

    [TestMethod]
    public void Test_BasicUnaryGates()
    {
        UnaryGate identity = new IdentityGate();
        UnaryGate pauliX = new PauliXGate();
        UnaryGate pauliZ = new PauliZGate();

        // Identity Zero 
        var sameZero = new QuBit(BasisState.Zero);
        sameZero.Apply(identity);
        Assert.IsTrue(sameZero.Measure() == 0);

        // Identity One 
        var sameOne = new QuBit(BasisState.One);
        sameOne.Apply(identity);
        Assert.IsTrue(sameOne.Measure() == 1);

        // Negate Zero
        var zero = new QuBit(BasisState.Zero);
        zero.Apply(pauliX);
        Assert.IsTrue(zero.Measure() == 1);

        // Negate One 
        var one = new QuBit(BasisState.One);
        one.Apply(pauliX);
        Assert.IsTrue(one.Measure()== 0);

        // Pauli Z on zero 
        zero = new QuBit(BasisState.Zero);
        zero.Apply(pauliZ);
        Assert.AreEqual(new Complex(1, 0), zero.Tensor[0]);
        Assert.AreEqual(new Complex(0, 0), zero.Tensor[1]);
        Assert.IsTrue(zero.Measure() == 0);

        // Pauli Z on one 
        one = new QuBit(BasisState.One);
        one.Apply(pauliZ);
        Assert.AreEqual(new Complex(0, 0), one.Tensor[0]);
        Assert.AreEqual(new Complex(-1, 0), one.Tensor[1]);
        Assert.IsTrue(one.Measure() == 1);
    }

    [TestMethod]
    public void Test_HadamardGate()
    {
        UnaryGate hadamard = new HadamardGate();

        int HadamardZero()
        {
            var zero = new QuBit(BasisState.Zero);
            zero.Apply(hadamard);
            return zero.Measure();
        }

        int HadamardOne()
        {
            var one = new QuBit(BasisState.One);
            one.Apply(hadamard);
            return one.Measure();
        }

        Driver.RunUnary(HadamardZero);
        Driver.RunUnary(HadamardOne);
    }
}
