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
            if (Debugger.IsAttached) { Debugger.Break(); }
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
            var controlledParameters = new GateParameters() { BaseGateKey = HadamardGate.Key };
            foreach (string gateKey in gates.Keys)
            {
                Gate gate =
                    (gateKey == ControlledGate.Key) || (gateKey == FlippedControlledGate.Key) ?
                        GateFactory.Produce(gateKey, controlledParameters) :
                        GateFactory.Produce(gateKey, defaultGateParameters);
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

    [TestMethod]
    public void Test_ApplyUnaryGate()
    {
        try
        {
            var identityMatrix = GateFactory.Produce(IdentityGate.Key).Matrix;
            // Unary gates 
            foreach (string gateCaptionKey in
                new string[] { "X", "Y", "Z", "H", "T", "S", "SX" })
            {
                var gate = GateFactory.Produce(gateCaptionKey);
                for (int qubitCount = 4; qubitCount <= 8; qubitCount++)
                {
                    Debug.WriteLine(string.Format("Gate: {0} - Qubits: {1} ", gateCaptionKey, qubitCount));
                    var registerSource = new QuRegister(qubitCount);
                    var matrix = gate.Matrix;
                    for (int i = 1; i < qubitCount; i++)
                    {
                        matrix = matrix.KroneckerProduct(identityMatrix);
                    }


                    var newState = matrix.Multiply(registerSource.State);
                    // var finalState = swap.Multiply(newState);
                    // Assert.IsTrue(finalState.IsAlmostEqualTo(registerSource.State));

                    Debug.WriteLine(registerSource.State.ToString());
                    Debug.WriteLine(newState.ToString());

                    //var clone = registerSource.DeepClone();
                    //var ketMap = new KetMap(qubitCount);
                    //clone.Swap(ketMap, j, k);
                    //Debug.WriteLine(clone.ToString());
                    //Assert.IsTrue(clone.State.IsAlmostEqualTo(newState));
                    //clone.Swap(ketMap, j, k);
                    //Assert.IsTrue(clone.State.IsAlmostEqualTo(registerSource.State));
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

/*
Gate: X - Qubits: 3

000   a   <0.829769; 0.558107>   <-0.592318; 0.805704>      e
001   b   <0.441098; 0.897459>    <-0.91512; 0.403183>      f
010   c   <0.668347; 0.743849>   <-0.771147; 0.636657>      g
011   d   <0.208571; 0.978007>    <-0.985874; 0.16749>      h

100   e  <-0.592318; 0.805704>    <0.829769; 0.558107>      a
101   f   <-0.91512; 0.403183>    <0.441098; 0.897459>      b
110   g  <-0.771147; 0.636657>    <0.668347; 0.743849>      c
111   h  <-0.985874; 0.16749>    <0.208571; 0.978007>       d

Apply X on 0-00 and 1-00
Apply X on 0-01 and 1-01
Apply X on 0-10 and 1-10
Apply X on 0-11 and 1-11

 */

/*
Gate: X - Qubits: 4             

0000   a   <0.2982; 0.954503>           <0.80153; -0.597955>       i 
0001   b   <-0.981364; -0.192157>       <0.144277; 0.989537>       j
0010   c   <0.471713; 0.881752>         <0.675337; -0.737509>      k
0011   d   <-0.999988; -0.00493259>     <0.327077; 0.944998>       l
                                                                   
0100   e   <-0.873557; 0.486722>        <0.748949; 0.662627>       m             
0101   f   <-0.0121682; -0.999926>      <-0.939266; 0.343189>      n               
0110   g   <-0.766925; 0.641737>        <0.859812; 0.51061>        o             
0111   h   <-0.199253; -0.979948>       <-0.858358; 0.513052>      p              
                                                                   
1000   i   <0.80153; -0.597955>         <0.2982; 0.954503>         a            
1001   j   <0.144277; 0.989537>         <-0.981364; -0.192157>     b            
1010   k   <0.675337; -0.737509>        <0.471713; 0.881752>       c             
1011   l   <0.327077; 0.944998>         <-0.999988; -0.00493259>   d
                                                                   
1100   m   <0.748949; 0.662627>         <-0.873557; 0.486722>      e
1101   n   <-0.939266; 0.343189>        <-0.0121682; -0.999926>    f
1110   o   <0.859812; 0.51061>          <-0.766925; 0.641737>      g
1111   p   <-0.858358; 0.513052>        <-0.199253; -0.979948>     h


*/