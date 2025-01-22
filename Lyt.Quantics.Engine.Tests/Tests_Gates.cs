using Lyt.Quantics.Engine.Gates.Base;

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
    public void Test_ApplyUnaryGateOnQuBitZero()
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
                    Debug.WriteLine(registerSource.State.ToString());
                    Debug.WriteLine(newState.ToString());
                    var clone = registerSource.DeepClone();
                    clone.ApplyUnaryGateOnQuBitZero(gate);
                    Debug.WriteLine(clone.ToString());
                    Assert.IsTrue(clone.State.IsAlmostEqualTo(newState));
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
    public void Test_ApplyUnaryGateWithSwaps()
    {
        try
        {
            var identityMatrix = GateFactory.Produce(IdentityGate.Key).Matrix;
            // Unary gates 
            foreach (string gateCaptionKey in
                new string[] { "X", "Y", "Z", "H", "T", "S", "SX" })
            {
                var gate = GateFactory.Produce(gateCaptionKey);
                var gateMatrix = gate.Matrix;

                for (int qubitCount = 4; qubitCount <= 8; qubitCount++)
                {
                    var ketMap = new KetMap(qubitCount);
                    // TODO :
                    // Fails if position is zero because regular matrix is not properly calculated 
                    //
                    for (int position = 1; position < qubitCount; position++)
                    {
                        Debug.WriteLine(string.Format("Gate: {0} - Qubits: {1} ", gateCaptionKey, qubitCount));
                        var registerSource = new QuRegister(qubitCount);
                        var matrix = identityMatrix;
                        for (int i = 1; i < position; i++)
                        {
                            matrix = matrix.KroneckerProduct(identityMatrix);
                        }

                        matrix = matrix.KroneckerProduct(gateMatrix);

                        for (int i = position + 1 ; i < qubitCount; i++)
                        {
                            matrix = matrix.KroneckerProduct(identityMatrix);
                        }

                        var newState = matrix.Multiply(registerSource.State);
                        Debug.WriteLine(registerSource.State.ToString());
                        Debug.WriteLine(newState.ToString());
                        var clone = registerSource.DeepClone();
                        clone.ApplyUnaryGateAtPosition(gate, ketMap, position);
                        Debug.WriteLine(clone.ToString());
                        Assert.IsTrue(clone.State.IsAlmostEqualTo(newState));
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
    public void Test_ApplyBinaryControlledGateOnQuBitsZeroOne()
    {
        try
        {
            var identityMatrix = GateFactory.Produce(IdentityGate.Key).Matrix;
            // Unary gates used to create a controlled one 
            foreach (string gateCaptionKey in
                new string[] { "X", "Y", "Z", "H", "T", "S", "SX" })
            {
                var baseGate = GateFactory.Produce(gateCaptionKey);
                var gate = new ControlledGate(baseGate);
                for (int qubitCount = 3; qubitCount <= 8; qubitCount++)
                {
                    Debug.WriteLine(string.Format("Gate: {0} - Qubits: {1} ", gateCaptionKey, qubitCount));
                    var registerSource = new QuRegister(qubitCount);
                    var matrix = gate.Matrix;
                    for (int i = 1; i < qubitCount-1; i++)
                    {
                        matrix = matrix.KroneckerProduct(identityMatrix);
                    }

                    var newState = matrix.Multiply(registerSource.State);
                    Debug.WriteLine(registerSource.State.ToString());
                    Debug.WriteLine(newState.ToString());

                    var clone = registerSource.DeepClone();
                    clone.ApplyBinaryControlledGateOnQuBitZero(gate);
                    Debug.WriteLine(clone.ToString());
                    Assert.IsTrue(clone.State.IsAlmostEqualTo(newState));
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
    public void Test_ApplyBinaryControlledGateAtPosition()
    {
        try
        {
            var identityMatrix = GateFactory.Produce(IdentityGate.Key).Matrix;
            // Unary gates used to create a controlled one 
            foreach (string gateCaptionKey in
                new string[] { "X", "Y", "Z", "H", "T", "S", "SX" })
            {
                var baseGate = GateFactory.Produce(gateCaptionKey);
                var gate = new ControlledGate(baseGate);
                var gateMatrix = gate.Matrix;
                for (int qubitCount = 3; qubitCount <= 8; qubitCount++)
                {
                    var ketMap = new KetMap(qubitCount);
                    for (int position = 0; position < qubitCount; position++)
                    {
                        // TODO : FIXME : Regular matrix is improperly calculated 
                        //
                        //
                        //Debug.WriteLine(string.Format("Gate: {0} - Qubits: {1} ", gateCaptionKey, qubitCount));
                        //var registerSource = new QuRegister(qubitCount);
                        //var matrix = identityMatrix;
                        //for (int i = 1; i < position; i++)
                        //{
                        //    matrix = matrix.KroneckerProduct(identityMatrix);
                        //}

                        //matrix = matrix.KroneckerProduct(gateMatrix);

                        //// Note +2 as matrix is from a binary gate and takes two slots 
                        //for (int i = position + 2; i < qubitCount; i++)
                        //{
                        //    matrix = matrix.KroneckerProduct(identityMatrix);
                        //}

                        //var newState = matrix.Multiply(registerSource.State);
                        //Debug.WriteLine(registerSource.State.ToString());
                        //Debug.WriteLine(newState.ToString());

                        //var clone = registerSource.DeepClone();
                        //clone.ApplyBinaryControlledGateAtPosition(gate, ketMap, position);
                        //Debug.WriteLine(clone.ToString());
                        //Assert.IsTrue(clone.State.IsAlmostEqualTo(newState));
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


/*
Gate: CX - Qubits: 3 

000   a   <-0.377787; 0.925893>     <-0.377787; 0.925893>   a
001   b  <-0.956883; -0.290474>    <-0.956883; -0.290474>   b 
010   c  <-0.997131; 0.0756959>    <-0.997131; 0.0756959>   c
011   d  <-0.167656; -0.985846>    <-0.167656; -0.985846>   d

100   e   <0.752394; -0.658713>      <0.925472; 0.378816>   g
101   f     <0.72552; 0.688201>     <-0.291538; 0.956559>   h
110   g    <0.925472; 0.378816>     <0.752394; -0.658713>   e
111   h   <-0.291538; 0.956559>       <0.72552; 0.688201>   f


Gate: CX - Qubits: 4 

0000   a  <-0.892545; -0.450959>   <-0.892545; -0.450959>   a
0001   b   <-0.849248; 0.527994>    <-0.849248; 0.527994>   b
0010   c   <-0.840801; 0.541344>    <-0.840801; 0.541344>   c
0011   d    <0.0253125; 0.99968>     <0.0253125; 0.99968>   d
          
0100   e  <-0.282793; -0.959181>   <-0.282793; -0.959181>   e                   
0101   f  <-0.966386; -0.257095>   <-0.966386; -0.257095>   f                   
0110   g  <-0.970327; -0.241797>   <-0.970327; -0.241797>   g                   
0111   h   <-0.711013; 0.703179>    <-0.711013; 0.703179>   h                   
          
1000   i  <-0.975811; -0.218618>   <-0.509107; -0.860704>    m                   
1001   j   <-0.694063; 0.719914>  <-0.999921; -0.0125731>    n                   
1010   k   <-0.682604; 0.730789>  <-0.999995; 0.00322427>    o                   
1011   l    <0.269387; 0.963032>    <-0.517131; 0.855906>    p                   
          
1100   m   <-0.509107; -0.860704>  <-0.975811; -0.218618     i
1101   n  <-0.999921; -0.0125731>   <-0.694063; 0.719914     j
1110   o  <-0.999995; 0.00322427>   <-0.682604; 0.730789     k
1111   p    <-0.517131; 0.855906>    <0.269387; 0.963032     l

Top half Unchanged 

Apply X at 0 on bottom half 

 */




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