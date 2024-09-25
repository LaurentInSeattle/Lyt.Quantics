namespace Lyt.Quantics.Engine.Tests;

[TestClass]
public sealed class Tests_Core
{
    #region Fixtures ( Not used for now) 

    //[AssemblyInitialize]
    //public static void AssemblyInit(TestContext context)
    //{
    //    // This method is called once for the test assembly, before any tests are run.
    //}

    //[AssemblyCleanup]
    //public static void AssemblyCleanup()
    //{
    //    // This method is called once for the test assembly, after all tests are run.
    //}

    //[ClassInitialize]
    //public static void ClassInit(TestContext context)
    //{
    //    // This method is called once for the test class, before any tests of the class are run.
    //}

    //[ClassCleanup]
    //public static void ClassCleanup()
    //{
    //    // This method is called once for the test class, after all tests of the class are run.
    //}

    //[TestInitialize]
    //public void TestInit()
    //{
    //    // This method is called before each test method.
    //}

    //[TestCleanup]
    //public void TestCleanup()
    //{
    //    // This method is called after each test method.
    //}

    #endregion Fixtures 

    [TestMethod]
    public void Test_Tooling()
    {
        var gates = GateFactory.AvailableProducts;
        Assert.IsTrue(gates is not null );
        var gate = GateFactory.Produce("H");
        Assert.IsTrue(gate is not null);
        Assert.IsTrue(gate.Name is not null);

        try
        {
            string serialized = SerializationUtilities.Serialize(QuComputer.Example);
            // Debug.WriteLine(serialized);
            var computer = SerializationUtilities.Deserialize<QuComputer>(serialized);
            Assert.IsTrue(computer is not null);
            serialized = SerializationUtilities.LoadEmbeddedTextResource("Entanglement.json");
            computer = SerializationUtilities.Deserialize<QuComputer>(serialized);
            Assert.IsTrue(computer is not null);
        }
        catch 
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void Test_Basics()
    {
        var q1 = new QuBit(QuState.Zero);
        int q1M = q1.Measure();
        Assert.IsTrue(q1M == 0);
        var q2 = new QuBit(QuState.One);
        int q2M = q2.Measure();
        Assert.IsTrue(q2M == 1);
        var qr = new QuRegister(q1, q2);
        var qrM = qr.Measure();
        Assert.IsTrue(qrM[0] == 0);
        Assert.IsTrue(qrM[1] == 1);
    }

    [TestMethod]
    public void Test_BasicUnaryGates()
    {
        Gate identity = new IdentityGate();
        Gate pauliX = new PauliXGate();
        Gate pauliZ = new PauliZGate();

        // Identity Zero 
        var sameZero = new QuBit(QuState.Zero);
        sameZero.Apply(identity);
        Assert.IsTrue(sameZero.Measure() == 0);

        // Identity One 
        var sameOne = new QuBit(QuState.One);
        sameOne.Apply(identity);
        Assert.IsTrue(sameOne.Measure() == 1);

        // Negate Zero
        var zero = new QuBit(QuState.Zero);
        zero.Apply(pauliX);
        Assert.IsTrue(zero.Measure() == 1);

        // Negate One 
        var one = new QuBit(QuState.One);
        one.Apply(pauliX);
        Assert.IsTrue(one.Measure()== 0);

        // Pauli Z on zero 
        zero = new QuBit(QuState.Zero);
        zero.Apply(pauliZ);
        Assert.AreEqual(new Complex(1, 0), zero.State[0]);
        Assert.AreEqual(new Complex(0, 0), zero.State[1]);
        Assert.IsTrue(zero.Measure() == 0);

        // Pauli Z on one 
        one = new QuBit(QuState.One);
        one.Apply(pauliZ);
        Assert.AreEqual(new Complex(0, 0), one.State[0]);
        Assert.AreEqual(new Complex(-1, 0), one.State[1]);
        Assert.IsTrue(one.Measure() == 1);

        // Hadamard on |0> should be |+>
        Gate hadamard = new HadamardGate();
        zero = new QuBit(QuState.Zero);
        var plus = new QuBit(QuState.Plus);
        zero.Apply(hadamard);
        Assert.AreEqual(zero, plus);

        // Hadamard on |1> should be |->
        one = new QuBit(QuState.One);
        var minus = new QuBit(QuState.Minus);
        one.Apply(hadamard);
        Assert.AreEqual(one, minus);
    }

    [TestMethod]
    public void Test_HadamardGate()
    {
        Gate hadamard = new HadamardGate();

        int HadamardZero()
        {
            var zero = new QuBit(QuState.Zero);
            zero.Apply(hadamard);
            return zero.Measure();
        }

        int HadamardOne()
        {
            var one = new QuBit(QuState.One);
            one.Apply(hadamard);
            return one.Measure();
        }

        Driver.RunUnary(HadamardZero);
        Driver.RunUnary(HadamardOne);
    }
}
