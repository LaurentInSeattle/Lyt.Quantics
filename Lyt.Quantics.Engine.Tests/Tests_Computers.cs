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
    public void Test_Machines()
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
}
