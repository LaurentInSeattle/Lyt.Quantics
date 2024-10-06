namespace Lyt.Quantics.Engine.Tests;

[TestClass]
public sealed class Tests_Computers
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
    public void Test_MachinesSingleRun()
    {
        try
        {
            static void ValidateBuildAndRun(string resourceFileName)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(resourceFileName));
                resourceFileName += ".json";
                string serialized = SerializationUtilities.LoadEmbeddedTextResource(resourceFileName);
                Assert.IsFalse(string.IsNullOrWhiteSpace(serialized));
                var computer = SerializationUtilities.Deserialize<QuComputer>(serialized);
                Assert.IsTrue(computer is not null);
                Debug.WriteLine(" ");
                Debug.WriteLine(" ");
                Debug.WriteLine("Processing:  " + computer.Name + "  from:  " + resourceFileName);
                bool isValid = computer.Validate(out string message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Debug.WriteLine(message);
                }
                Assert.IsTrue(isValid);

                bool isBuilt = computer.Build(out message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Debug.WriteLine(message);
                }
                Assert.IsTrue(isBuilt);

                bool isPrepared = computer.Prepare(out message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Debug.WriteLine(message);
                }
                Assert.IsTrue(isPrepared);

                bool isComplete = computer.Run(out message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Debug.WriteLine(message);
                }
                Assert.IsTrue(isComplete);
                Debug.WriteLine(computer.Name + " - Final result: " + computer.Result);
            }

            ValidateBuildAndRun("FullAdder");
            ValidateBuildAndRun("Deutsch_Balanced");
            ValidateBuildAndRun("Deutsch_Constant");
            ValidateBuildAndRun("RxyzCnot_Test");
            ValidateBuildAndRun("Rxyz_Test");
            ValidateBuildAndRun("SX_Test");
            ValidateBuildAndRun("Toffoli_Basic");
            ValidateBuildAndRun("HX_PhaseFlip");
            ValidateBuildAndRun("Entanglement");
            ValidateBuildAndRun("EntanglementNot");
            ValidateBuildAndRun("EntanglementFlipped");
            ValidateBuildAndRun("HX_Swap");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }

    [TestMethod]
    public void Test_MachinesManyRuns()
    {
        try
        {
            static QuComputer ValidateAndBuild(string resourceFileName)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(resourceFileName));
                resourceFileName += ".json";
                string serialized = SerializationUtilities.LoadEmbeddedTextResource(resourceFileName);
                Assert.IsFalse(string.IsNullOrWhiteSpace(serialized));
                var computer = SerializationUtilities.Deserialize<QuComputer>(serialized);
                Assert.IsTrue(computer is not null);
                Debug.WriteLine(" ");
                Debug.WriteLine(" ");
                Debug.WriteLine("Processing:  " + computer.Name + "  from:  " + resourceFileName);
                bool isValid = computer.Validate(out string message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Debug.WriteLine(message);
                }
                Assert.IsTrue(isValid);

                bool isBuilt = computer.Build(out message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Debug.WriteLine(message);
                }
                Assert.IsTrue(isBuilt);

                return computer;
            }

            static void PrepareAndRun(
                QuComputer computer, List<List<QuState>> states, List<List<double>> expected)
            {
                Assert.IsTrue(computer is not null);
                Assert.IsTrue(states.Count == expected.Count);
                Debug.WriteLine(" ");
                Debug.WriteLine(" ");
                Debug.WriteLine("Batch Processing:  " + computer.Name);

                for (int i = 0; i < states.Count;  i++)
                {
                    bool isPrepared = computer.Prepare(states[i], expected[i], out string message);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        Debug.WriteLine(message);
                    }

                    Assert.IsTrue(isPrepared);

                    bool isComplete = computer.Run(out message);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        Debug.WriteLine(message);
                    }

                    Assert.IsTrue(isComplete);
                    Debug.WriteLine(computer.Name + " - Final result: " + computer.Result);
                }
            }

            static void ManyRuns(
                string resourceFileName, List<List<QuState>> states, List<List<double>> expected)
            {
                var adder = ValidateAndBuild(resourceFileName);
                PrepareAndRun(adder, states, expected);
            }

            List<List<QuState>> adderStates =
            [
                [QuState.Zero, QuState.Zero, QuState.Zero, QuState.Zero],
                [QuState.Zero, QuState.One, QuState.Zero, QuState.Zero],
                [QuState.Zero, QuState.Zero, QuState.One, QuState.Zero],
                [QuState.Zero, QuState.One, QuState.One, QuState.Zero],

                [QuState.One, QuState.Zero, QuState.Zero, QuState.Zero],
                [QuState.One, QuState.One, QuState.Zero, QuState.Zero],
                [QuState.One, QuState.Zero, QuState.One, QuState.Zero],
                [QuState.One, QuState.One, QuState.One, QuState.Zero],
            ];
            List<List<double>> adderExpected =
            [
                [ 1.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 1.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  1.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 1.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],

                [ 0.0, 0.0, 0.0, 0.0,  0.0, 1.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 1.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 1.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 1.0 ],
            ];
            ManyRuns("FullAdder", adderStates, adderExpected); 

            List<List<QuState>> orGateStates =
            [
                [QuState.Zero, QuState.Zero, QuState.Zero],
                [QuState.Zero, QuState.One, QuState.Zero ],
                [QuState.One, QuState.Zero, QuState.Zero ],
                [QuState.One, QuState.One, QuState.Zero ],
            ];
            List<List<double>> orGateExpected =
            [
                [ 0.0, 0.0, 0.0, 1.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  0.0, 1.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 1.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  1.0, 0.0, 0.0, 0.0 ],
            ];
            ManyRuns("OR_Gate", orGateStates, orGateExpected);

            List<List<QuState>> andGateStates =
            [
                [QuState.Zero, QuState.Zero, QuState.Zero],
                [QuState.Zero, QuState.One, QuState.Zero ],
                [QuState.One, QuState.Zero, QuState.Zero ],
                [QuState.One, QuState.One, QuState.Zero ],
            ];
            List<List<double>> andGateExpected =
            [
                [ 1.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 1.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 1.0, 0.0, 0.0,  0.0, 0.0, 0.0, 0.0 ],
                [ 0.0, 0.0, 0.0, 0.0,  0.0, 0.0, 0.0, 1.0 ],
            ];
            ManyRuns("Toffoli_Basic", andGateStates, andGateExpected);

            List<List<QuState>> teleporterStates =
            [
                [QuState.Zero, QuState.Zero, QuState.Zero],
                [QuState.One, QuState.Zero, QuState.Zero ],
            ];
            List<List<double>> teleporterExpected =
            [
                [0.0, 0.0, 0.25, 0.25, 0.25, 0.25, 0.0, 0.0],
                [ 0.25, 0.25, 0.0, 0.0, 0.0, 0.0, 0.25, 0.25 ],
            ];
            ManyRuns("Teleport", teleporterStates, teleporterExpected);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }
}
