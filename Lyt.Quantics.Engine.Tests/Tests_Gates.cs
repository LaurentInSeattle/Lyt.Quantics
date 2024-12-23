﻿namespace Lyt.Quantics.Engine.Tests;

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

    private static void VerifyMatrix(Matrix<Complex> matrix)
    {
        // Debug.WriteLine(matrix);
        int dimension = matrix.RowCount;
        var dagger = matrix.ConjugateTranspose();
        var shouldBeIdentity = matrix.Multiply(dagger);
        var trueIdentity = Matrix<Complex>.Build.DenseIdentity(dimension, dimension);
        double tolerance = MathUtilities.Epsilon;
        if (!shouldBeIdentity.AlmostEqual(trueIdentity, tolerance))
        {
            Debug.WriteLine("Matrix is not unitary.");
            Debug.WriteLine(matrix);
            Debug.WriteLine("shouldBeIdentity: " + shouldBeIdentity);
            Debug.WriteLine("trueIdentity: " + trueIdentity);
            if( Debugger.IsAttached ) { Debugger.Break(); }
            Assert.Fail();
        }
    }

    [TestMethod]
    public void Test_GateDefinitions()
    {
        try
        {
            var gates = GateFactory.AvailableProducts;
            Assert.IsTrue(gates is not null);
            var defaultGateParameters = new GateParameters();
            foreach (string gateKey in gates.Keys)
            {
                var gate = GateFactory.Produce(gateKey, defaultGateParameters);
                Assert.IsTrue(gate is not null);
                Assert.IsTrue(gate.Name is not null);
                int dimension = gate.MatrixDimension;
                Debug.WriteLine(gate.CaptionKey + ":  " + gate.Name + "  Dim: " + dimension.ToString());
                Assert.IsTrue(gate.QuBitsTransformed == gate.ControlQuBits + gate.TargetQuBits);

                VerifyMatrix(gate.Matrix); 
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
            foreach (Axis axis in new Axis[] { Axis.X, Axis.Y, Axis.Z })
            {
                for (double theta = 0; theta <= Math.Tau; theta += Math.PI / 6)
                {
                    var parameters = new GateParameters()
                    {
                        Angle = theta,
                        Axis = axis,
                        IsPiDivisor = false, 
                    };
                    var gate = new RotationGate(parameters);
                    int dimension = gate.MatrixDimension;

                    VerifyMatrix(gate.Matrix);

                    parameters = new GateParameters()
                    {
                        Angle = Math.Tau - theta,
                        Axis = axis,
                        IsPiDivisor = false,
                    };
                    var rotatedGate = new RotationGate(parameters);
                    var trueIdentity = Matrix<Complex>.Build.DenseIdentity(dimension, dimension);
                    var shouldBeIdentity = gate.Matrix.Multiply(rotatedGate.Matrix);
                    shouldBeIdentity = shouldBeIdentity.Multiply(-1);
                    if (!shouldBeIdentity.AlmostEqual(trueIdentity, MathUtilities.Epsilon))
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

    [TestMethod]
    public void Test_PhaseGates()
    {
        try
        {
            for (double lambda = 0; lambda <= Math.Tau; lambda += Math.PI / 6)
            {
                var parameters = new GateParameters()
                {
                    Angle = lambda,
                    IsPiDivisor = false,
                };
                var gate = new PhaseGate(parameters);
                int dimension = gate.MatrixDimension;

                VerifyMatrix(gate.Matrix); 
            }

            // Verify that:
            // P ( λ = π )     = Z
            // P ( λ = π / 2 ) = S
            // P ( λ = π / 4 ) = T
            static void Verify(double lambda, string captionKey)
            {
                var parameters = new GateParameters()
                {
                    Angle = lambda,
                    IsPiDivisor = false,
                };
                var gate = new PhaseGate(parameters);
                int dimension = gate.MatrixDimension;
                var checkGate = GateFactory.Produce(captionKey);
                double tolerance = MathUtilities.Epsilon;
                if (!gate.Matrix.AlmostEqual(checkGate.Matrix, tolerance))
                {
                    Debug.WriteLine("Phase Test");
                    Debug.WriteLine(gate.CaptionKey + ":  " + gate.Name + "  Dim: " + dimension.ToString());
                    Debug.WriteLine("Phase gate: " + gate);
                    Debug.WriteLine("Check: " + captionKey + checkGate);
                    Assert.Fail();
                }
            }

            Verify(Math.PI, "Z");
            Verify(Math.PI / 2.0, "S");
            Verify(Math.PI / 4.0, "T");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }

    [TestMethod]
    public void Test_SwapGate()
    {
        try
        {
            // TODO 

            // Verify that:
            // Swap == CNOT Flipped CNOT CNOT 

            //var gate = new ControlledGate(srcCaptionKey);
            //int dimension = gate.Dimension;
            //var checkGate = GateFactory.Produce(dstCaptionKey);
            //double tolerance = MathUtilities.Epsilon;
            //if (!gate.Matrix.AlmostEqual(checkGate.Matrix, tolerance))
            //{
            //    Debug.WriteLine("Controlled Test");
            //    Debug.WriteLine(gate.CaptionKey + ":  " + gate.Name + "  Dim: " + dimension.ToString());
            //    Debug.WriteLine("Phase gate: " + gate);
            //    Debug.WriteLine("Check: " + dstCaptionKey + checkGate);
            //    Assert.Fail();
            //}

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }

    [TestMethod]
    public void Test_ControlledGates()
    {
        try
        {
            foreach (string baseGateCaptionKey in
                new string[] { "I", "X", "Y", "Z", "H", "T", "S", "SX", "Swap", "CX", "CZ" })
            {
                var gate = new ControlledGate(baseGateCaptionKey);
                VerifyMatrix(gate.Matrix);
            }

            // Verify that:
            // Controlled Swap == CSwap (Fredkin ) 
            // Controlled CX == CCX (Toffoli)  
            // Controlled X == CX (CNot)  
            // Controlled Z == CZ 
            // Controlled S == CS 
            static void Verify(string srcCaptionKey, string dstCaptionKey)
            {
                var gate = new ControlledGate(srcCaptionKey);
                int dimension = gate.MatrixDimension;
                var checkGate = GateFactory.Produce(dstCaptionKey);
                double tolerance = MathUtilities.Epsilon;
                if (!gate.Matrix.AlmostEqual(checkGate.Matrix, tolerance))
                {
                    Debug.WriteLine("Controlled Test");
                    Debug.WriteLine(gate.CaptionKey + ":  " + gate.Name + "  Dim: " + dimension.ToString());
                    Debug.WriteLine("Phase gate: " + gate);
                    Debug.WriteLine("Check: " + dstCaptionKey + checkGate);
                    Assert.Fail();
                }
            }

            // Not the same dimensions, cant test that with the above 'Verify'  
            // Verify("I", "I");

            Verify("X", "CX");
            Verify("S", "CS");
            Verify("Z", "CZ");
            Verify("Swap", "CSwap");
            Verify("CX", "CCX");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }
}
