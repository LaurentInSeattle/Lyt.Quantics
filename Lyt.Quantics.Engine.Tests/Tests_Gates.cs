namespace Lyt.Quantics.Engine.Tests;

[TestClass]
public sealed class Tests_Gates
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
    public void Test_GateDefinitions()
    {
        try
        {
            var gates = GateFactory.AvailableProducts;
            Assert.IsTrue(gates is not null);
            foreach (string gateKey in gates.Keys)
            {
                var gate = GateFactory.Produce(gateKey);
                Assert.IsTrue(gate is not null);
                Assert.IsTrue(gate.Name is not null);
                int dimension = gate.Matrix.RowCount;
                Debug.WriteLine(gate.CaptionKey + ":  " + gate.Name + "  Dim: " + dimension.ToString());
                var dagger = gate.Matrix.ConjugateTranspose();
                var shouldBeIdentity = gate.Matrix.Multiply(dagger);
                var trueIdentity = Matrix<Complex>.Build.DenseIdentity(dimension, dimension);
                double tolerance = MathUtilities.Epsilon;
                if (!shouldBeIdentity.AlmostEqual(trueIdentity, tolerance))
                {
                    Debug.WriteLine("Unitary Test");
                    Debug.WriteLine("shouldBeIdentity: " + shouldBeIdentity);
                    Debug.WriteLine("trueIdentity: " + trueIdentity);
                    Assert.Fail();
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }

    [TestMethod]
    public void Test_RotationGates()
    {
        try
        {
            foreach (RotationGate.Axis axis in
                new RotationGate.Axis[] { RotationGate.Axis.X, RotationGate.Axis.Y, RotationGate.Axis.Z })
            {
                for (double theta = 0; theta <= Math.Tau; theta += Math.PI / 6)
                {
                    var gate = new RotationGate(axis, theta);
                    int dimension = gate.Matrix.RowCount;
                    var dagger = gate.Matrix.ConjugateTranspose();
                    var shouldBeIdentity = gate.Matrix.Multiply(dagger);
                    var trueIdentity = Matrix<Complex>.Build.DenseIdentity(dimension, dimension);
                    double tolerance = MathUtilities.Epsilon;
                    if (!shouldBeIdentity.AlmostEqual(trueIdentity, tolerance))
                    {
                        Debug.WriteLine("Unitary Test");
                        Debug.WriteLine(gate.CaptionKey + ":  " + gate.Name + "  Dim: " + dimension.ToString());
                        Debug.WriteLine("shouldBeIdentity: " + shouldBeIdentity);
                        Debug.WriteLine("trueIdentity: " + trueIdentity);
                        Assert.Fail();
                    }

                    var rotatedGate = new RotationGate(axis, Math.Tau - theta);
                    shouldBeIdentity = gate.Matrix.Multiply(rotatedGate.Matrix);
                    shouldBeIdentity = shouldBeIdentity.Multiply(-1); 
                    if (!shouldBeIdentity.AlmostEqual(trueIdentity, tolerance))
                    {
                        Debug.WriteLine("Rotated Test");
                        Debug.WriteLine("shouldBeIdentity: " + shouldBeIdentity);
                        Debug.WriteLine("trueIdentity: " + trueIdentity);
                        Assert.Fail();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }
}
