namespace Lyt.Quantics.Engine.Tests;

[TestClass]
public sealed class Tests_Teleportation
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

    //[TestClass()]
    //public class TeleportationTest
    //{
    //    [TestMethod()]
    //    public void Teleportation_FalseQubit_Test()
    //    {
    //        var sender = new Qubit(false);
    //        var recv = new Qubit(false);

    //        var b = Teleport(sender, recv, false);

    //        Assert.IsFalse(b);
    //    }

    //    [TestMethod()]
    //    public void Teleportation_TrueQubit_Test()
    //    {
    //        var sender = new Qubit(false);
    //        var recv = new Qubit(false);

    //        var b = Teleport(sender, recv, true);

    //        Assert.IsTrue(b);
    //    }

    //    private static bool Teleport(Qubit sender, Qubit recv, bool message)
    //    {
    //        var msg = new Qubit(message);

    //        new HGate().Apply(sender);
    //        new CXGate().Apply(sender, recv);

    //        new CXGate().Apply(msg, sender);
    //        new HGate().Apply(msg);

    //        var bMsg = msg.Measure();
    //        var bSender = sender.Measure();

    //        if (bSender)
    //            new XGate().Apply(recv);
    //        if (bMsg)
    //            new ZGate().Apply(recv);

    //        var bRecv = recv.Measure();
    //        if (message != bRecv)
    //            Assert.Fail();

    //        return bRecv;
    //    }
    //}

    [TestMethod]
    public void Test_MachinesManyRuns()
    {
        try
        {
            //static QuComputer ValidateAndBuild(string resourceFileName)
            //{
            //    Assert.IsFalse(string.IsNullOrWhiteSpace(resourceFileName));
            //    resourceFileName += ".json";
            //    string serialized = SerializationUtilities.LoadEmbeddedTextResource(resourceFileName);
            //    Assert.IsFalse(string.IsNullOrWhiteSpace(serialized));
            //    var computer = SerializationUtilities.Deserialize<QuComputer>(serialized);
            //    Assert.IsTrue(computer is not null);
            //    Debug.WriteLine(" ");
            //    Debug.WriteLine(" ");
            //    Debug.WriteLine("Processing:  " + computer.Name + "  from:  " + resourceFileName);
            //    bool isValid = computer.Validate(out string message);
            //    if (!string.IsNullOrWhiteSpace(message))
            //    {
            //        Debug.WriteLine(message);
            //    }
            //    Assert.IsTrue(isValid);

            //    bool isBuilt = computer.Build(out message);
            //    if (!string.IsNullOrWhiteSpace(message))
            //    {
            //        Debug.WriteLine(message);
            //    }
            //    Assert.IsTrue(isBuilt);

            //    return computer;
            //}

            //static void PrepareAndRun(
            //    QuComputer computer, List<List<QuState>> states, List<List<double>> expected)
            //{
            //    Assert.IsTrue(computer is not null);
            //    Assert.IsTrue(states.Count == expected.Count);
            //    Debug.WriteLine(" ");
            //    Debug.WriteLine(" ");
            //    Debug.WriteLine("Batch Processing:  " + computer.Name);

            //    for (int i = 0; i < states.Count; i++)
            //    {
            //        bool isPrepared = computer.Prepare(states[i], expected[i], out string message);
            //        if (!string.IsNullOrWhiteSpace(message))
            //        {
            //            Debug.WriteLine(message);
            //        }

            //        Assert.IsTrue(isPrepared);

            //        bool isComplete = computer.Run(out message);
            //        if (!string.IsNullOrWhiteSpace(message))
            //        {
            //            Debug.WriteLine(message);
            //        }

            //        Assert.IsTrue(isComplete);
            //        Debug.WriteLine(computer.Name + " - Final result: " + computer.Result);
            //    }
            //}

            //var orGate = ValidateAndBuild("OR_Gate");
            //List<List<QuState>> orGateStates =
            //[
            //    [QuState.Zero, QuState.Zero, QuState.Zero],
            //    [QuState.Zero, QuState.One, QuState.Zero ],
            //    [QuState.One, QuState.Zero, QuState.Zero ],
            //    [QuState.One, QuState.One, QuState.Zero ],
            //];
            //List<List<double>> orGateExpected =
            //[
            //    [ 0.0, 0.0, 0.0, 1.0,  0.0, 0.0, 0.0, 0.0 ],
            //    [ 0.0, 0.0, 0.0, 0.0,  0.0, 1.0, 0.0, 0.0 ],
            //    [ 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 1.0, 0.0 ],
            //    [ 0.0, 0.0, 0.0, 0.0,  1.0, 0.0, 0.0, 0.0 ],
            //];

            //PrepareAndRun(orGate, orGateStates, orGateExpected);

            //var andGate = ValidateAndBuild("Toffoli_Basic");
            //List<List<QuState>> andGateStates =
            //[
            //    [QuState.Zero, QuState.Zero, QuState.Zero],
            //    [QuState.Zero, QuState.One, QuState.Zero ],
            //    [QuState.One, QuState.Zero, QuState.Zero ],
            //    [QuState.One, QuState.One, QuState.Zero ],
            //];
            //List<List<double>> andGateExpected =
            //[
            //    [ 1.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
            //    [ 0.0, 0.0, 1.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
            //    [ 0.0, 1.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
            //    [ 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 1.0 ],
            //];

            //PrepareAndRun(andGate, andGateStates, andGateExpected);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }
}