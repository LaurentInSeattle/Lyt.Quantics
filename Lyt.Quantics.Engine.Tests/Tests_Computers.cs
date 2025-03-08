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
            SwapData.Load();

            static void BuildAndRun(
                QuComputer computer, bool runUsingKroneckerProduct, QuRegister? initialState = null)
            {
                computer.RunUsingKroneckerProduct = runUsingKroneckerProduct;
                bool isBuilt = computer.Build(out string message);
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

                if (initialState is not null)
                {
                    computer.Initialize(initialState);
                }

                bool isComplete = computer.Run(checkExpected: true, null, out message);
                if (!string.IsNullOrWhiteSpace(message))
                {
                    Debug.WriteLine(message);
                }
                Assert.IsTrue(isComplete);
                Debug.WriteLine(
                    computer.Name +
                    "\n   Final Register: " + computer.FinalRegister.ToString());
            }

            static QuComputer ValidateBuildAndRun(string resourceFileName, QuRegister? initialState = null)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(resourceFileName));
                resourceFileName += ".json";
                string serialized = SerializationUtilities.LoadEmbeddedTextResource(resourceFileName, out string? resourceFullName);
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

                BuildAndRun(computer, runUsingKroneckerProduct:true , initialState);
                var result1 = computer.FinalRegister.DeepClone();
                BuildAndRun(computer, runUsingKroneckerProduct: false, initialState);
                var result2 = computer.FinalRegister.DeepClone();

                if (!result1.IsAlmostEqualTo(result2))
                {
                    Assert.Fail();
                } 

                return computer;
            }

            ValidateBuildAndRun("Entanglement");
            ValidateBuildAndRun("Rxyz_Test");
            ValidateBuildAndRun("RxyzCnot_Test");
            ValidateBuildAndRun("Full Adder - with implicit swaps");
            ValidateBuildAndRun("Deutsch_Balanced");
            ValidateBuildAndRun("Deutsch_Constant");
            ValidateBuildAndRun("SX_Test");
            ValidateBuildAndRun("Toffoli_Basic");
            ValidateBuildAndRun("HX_PhaseFlip");
            ValidateBuildAndRun("EntanglementNot");
            ValidateBuildAndRun("EntanglementFlipped");
            ValidateBuildAndRun("HX_Swap");

            var initialState1 = new QuRegister(4);
            var initialState2 = initialState1.DeepClone();
            var computer1 = ValidateBuildAndRun("Rotations_Single_Stage", initialState1);
            var computer2 = ValidateBuildAndRun("Rotations_Dual_Stage", initialState2);
            Assert.IsTrue(
                computer1.FinalRegister.IsAlmostEqualTo(computer2.FinalRegister));

            initialState1 = new QuRegister(4);
            initialState2 = initialState1.DeepClone();
            computer1 = ValidateBuildAndRun("Double_Ent_Three_Stages", initialState1);
            computer2 = ValidateBuildAndRun("Double_Ent_Five_Stages", initialState2);
            Assert.IsTrue(
                computer1.FinalRegister.IsAlmostEqualTo(computer2.FinalRegister));

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
            SwapData.Load(); 

            static QuComputer ValidateAndBuild(string resourceFileName)
            {
                Assert.IsFalse(string.IsNullOrWhiteSpace(resourceFileName));
                resourceFileName += ".json";
                string serialized = 
                    SerializationUtilities.LoadEmbeddedTextResource(resourceFileName, out string? resourceFullName);
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

                    bool isComplete = computer.Run(checkExpected: true, null, out message);
                    if (!string.IsNullOrWhiteSpace(message))
                    {
                        Debug.WriteLine(message);
                    }

                    Assert.IsTrue(isComplete);
                    Debug.WriteLine(computer.Name);
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
            ManyRuns("Full Adder - with implicit swaps", adderStates, adderExpected); 

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
                [0.25, 0.25, 0.25, 0.25, 0.0, 0.0, 0.0, 0.0],
                [0.0, 0.0, 0.0, 0.0, 0.25, 0.25, 0.25, 0.25 ],
            ];
            ManyRuns("Single Qbit Teleportation", teleporterStates, teleporterExpected);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }
}
