namespace Lyt.Quantics.Engine.Tests;

[TestClass]
public sealed class Tests_Matrices
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
    public void Test_SwapControlled()
    {
        try
        {
            var defaultParameters = new GateParameters();
            foreach (string key in new string[] { "H", "X", "S", "SX", "T", "Y", "Rx", "Ph" })
            {
                var testGate = GateFactory.Produce(key, defaultParameters);
                var swapGate = GateFactory.Produce(SwapGate.Key, defaultParameters);
                var controlledGate = new ControlledGate(testGate);
                var flippedControlledGate = new FlippedControlledGate(testGate);
                var swap = swapGate.Matrix;
                var controlled = controlledGate.Matrix;
                Debug.WriteLine(controlled);
                var m1 = swap.Multiply(controlled);
                var m2 = m1.Multiply(swap);
                Debug.WriteLine(m2);
                Assert.IsTrue(flippedControlledGate.Matrix.AlmostEqual(m2, MathUtilities.Epsilon));
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }

    [TestMethod]
    public void Test_SwapTensorProduct()
    {
        try
        {
            var swapGate = GateFactory.Produce(SwapGate.Key, new GateParameters());
            var identityGate = GateFactory.Produce(IdentityGate.Key, new GateParameters());

            var swap = swapGate.Matrix;
            var identity = identityGate.Matrix;

            var m1 = swap.KroneckerProduct(identity);
            var m2 = identity.KroneckerProduct(swap);

            var registerSource = new QuRegister(3);
            var registerClone = registerSource.DeepClone();

            var newState = m1.Multiply(registerSource.State);
            Debug.WriteLine(registerSource.State);
            Debug.WriteLine(m1);
            Debug.WriteLine(newState);

            newState = m2.Multiply(registerSource.State);
            Debug.WriteLine(registerSource.State);
            Debug.WriteLine(m2);
            Debug.WriteLine(newState);

            newState = m1.Multiply(registerSource.State);
            newState = m2.Multiply(newState);
            newState = m1.Multiply(newState);

            Debug.WriteLine(registerSource.State);
            Debug.WriteLine(newState);

            var mm1 = MatricesUtilities.SingleStageSwapMatrix(3, 0);
            Debug.WriteLine(mm1);
            Assert.IsTrue(mm1.AlmostEqual(m1, MathUtilities.Epsilon));
            var mm2 = MatricesUtilities.SingleStageSwapMatrix(3, 1);
            Debug.WriteLine(mm2);
            Assert.IsTrue(mm2.AlmostEqual(m2, MathUtilities.Epsilon));
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }

    [TestMethod]
    public void Test_NotTensorProduct()
    {
        try
        {
            var xGate = GateFactory.Produce("X");
            var identityGate = GateFactory.Produce(IdentityGate.Key);

            var not = xGate.Matrix;
            var identity = identityGate.Matrix;

            var m1 = not.KroneckerProduct(identity);
            var m2 = identity.KroneckerProduct(not);

            var registerSource = new QuRegister(2);
            var registerClone = registerSource.DeepClone();

            var newState = m1.Multiply(registerSource.State);
            Debug.WriteLine(registerSource.State);
            Debug.WriteLine(m1);
            Debug.WriteLine(newState);

            newState = m2.Multiply(registerSource.State);
            Debug.WriteLine(registerSource.State);
            Debug.WriteLine(m2);
            Debug.WriteLine(newState);

            var m3 = identity.KroneckerProduct(identity);
            Debug.WriteLine(m3);
            var m4 = m3.KroneckerProduct(not);
            Debug.WriteLine(m4);
            registerSource = new QuRegister(3);
            registerClone = registerSource.DeepClone();
            newState = m4.Multiply(registerSource.State);
            Debug.WriteLine(registerSource.State);
            Debug.WriteLine(m4);
            Debug.WriteLine(newState);

        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            Assert.Fail();
        }
    }

    [TestMethod]
    public void Test_Swaps()
    {
        for (int i = 2; i < 8; i++)
        {
            for (int j = 0; j < i; j++)
            {
                for (int k = j + 1; k < i; k++)
                {
                    Debug.WriteLine(string.Format("Qubits {0} Swap {1} - {2}", i, j, k));
                    var registerSource = new QuRegister(i);
                    var swap = MatricesUtilities.SwapMatrix(i, j, k);
                    var newState = swap.Multiply(registerSource.State);
                    var finalState = swap.Multiply(newState);
                    Assert.IsTrue(finalState.IsAlmostEqualTo(registerSource.State));
                    //Debug.WriteLine(registerSource.State.ToString());
                    //Debug.WriteLine(newState.ToString());
                }
            }
        }
    }

    [TestMethod]
    public void Test_IdentityKron()
    {
        var identityGate = GateFactory.Produce(IdentityGate.Key, new GateParameters());
        var identity = identityGate.Matrix;
        var m = identity;
        Debug.WriteLine(m);
        for (int i = 0; i < 3; i++)
        {
            m = m.KroneckerProduct(identity);
            Debug.WriteLine(m);
        }

        var xGate = GateFactory.Produce(PauliXGate.Key, new GateParameters());
        var x = xGate.Matrix;
        for (int qubits = 2; qubits < 5; qubits++)
        {
            var registerSource = new QuRegister(qubits);
            Debug.WriteLine(registerSource.State.ToString());
            m = x;
            for (int i = 1; i < qubits; i++)
            {
                m = m.KroneckerProduct(identity);
            }

            Debug.WriteLine(m);
            var newState = m.Multiply(registerSource.State);
            Debug.WriteLine(newState.ToString());
        }
    }

    [TestMethod]
    public void Test_Fast_Swaps()
    {
        SwapData.Poke(); 

        for (int qubitCount = 3; qubitCount <= 8; qubitCount++)
        {
            for (int j = 0; j < qubitCount; j++)
            {
                for (int k = j + 1; k < qubitCount; k++)
                {
                    Debug.WriteLine(string.Format("Qubits {0} Swap {1} - {2}", qubitCount, j, k));
                    var registerSource = new QuRegister(qubitCount);
                    var swap = MatricesUtilities.SwapMatrix(qubitCount, j, k);
                    var newState = swap.Multiply(registerSource.State);
                    var finalState = swap.Multiply(newState);
                    Assert.IsTrue(finalState.IsAlmostEqualTo(registerSource.State));

                    Debug.WriteLine(registerSource.State.ToString());
                    Debug.WriteLine(newState.ToString());

                    var clone = registerSource.DeepClone();
                    clone.GeneralSwap(j, k);
                    Debug.WriteLine(clone.ToString());
                    Assert.IsTrue(clone.State.IsAlmostEqualTo(newState));
                    clone.GeneralSwap(j, k);
                    Assert.IsTrue(clone.State.IsAlmostEqualTo(registerSource.State));
                }
            }
        }
    }
}

/*
 * 

Matrix M1 : Run for I at 2 , Swap at 0 1  

Bits 0 and 1 get swapped, bit string starting at zero 

000     a  <0.416252; 0.909249>        <0.416252; 0.909249>    a
001     b  <-0.753437; 0.65752>        <-0.753437; 0.65752>    b
010     c  <-0.797772; 0.602959>       <0.800567; -0.599243>   e   * 
011     d  <0.368802; 0.929508>        <0.786292; -0.617856>   f   * 
100     e  <0.800567; -0.599243>       <-0.797772; 0.602959>   c   * 
101     f  <0.786292; -0.617856>       <0.368802; 0.929508>    d   * 
110     g  <-0.975252; 0.221097>       <-0.975252; 0.221097>   g
111     h  <0.497383; 0.867531>        <0.497383; 0.867531>    h 


Matrix M2 : Run for I at 0 , Swap at 1 2 

Bits 1 and 2 get swapped, bit string starting at zero 

000     a   <-0.77722; 0.629229>        <-0.77722; 0.629229>        a
001     b   <-0.354704; -0.934979>      <-0.996955; 0.0779801>      c   * 
010     c   <-0.996955; 0.0779801>      <-0.354704; -0.934979>      b   * 
011     d   <0.367435; -0.930049>       <0.367435; -0.930049>       d  

100     e   <-0.0444433; 0.999012>      <-0.0444433; 0.999012>      e
101     f   <-0.773535; -0.633754>      <-0.923949; -0.382516>      g   *
110     g   <-0.923949; -0.382516>      <-0.773535; -0.633754>      f   *
111     h   <-0.932534; 0.361082>       <-0.932534; 0.361082>       h 
 

Matrix M1 x M2 x M1 : Run for Swap at 0 2  

Bits 0 and 2 get swapped, bit string starting at zero 

000     a   <-0.939101; 0.34364>       <-0.939101; 0.34364>     a
001     b   <0.344317; 0.938853>       <-0.879411; 0.476063>    e   *
010     c  <0.866865; -0.498542>       <0.866865; -0.498542>    c
011     d   <-0.99399; 0.109469>       <0.330961; 0.943644>     g   *
100     e  <-0.879411; 0.476063>       <0.344317; 0.938853>     b   *   
101     f  <0.999892; 0.0147022>       <0.999892; 0.0147022>    f
110     g   <0.330961; 0.943644>       <-0.99399; 0.109469>     d   *
111     h   <0.823723; 0.566992>       <0.823723; 0.566992>     h 

 * 
 */

/*
 * 

Qubits 4 Swap 0 - 1

0000    <0.222901; 0.974841>    <0.222901; 0.974841>  
0001    <-0.955037; 0.296486>   <-0.955037; 0.296486>  
0010    <-0.921024; -0.389505>   <-0.921024; -0.389505>  
0011    <0.31824; -0.94801>      <0.31824; -0.94801>  

0100    <0.948218; 0.317621>  <-0.939807; 0.341706>      *                   
0101    <-0.244493; 0.969651>  <-0.412279; -0.911058>    * 
0110    <-0.81231; 0.583226>  <0.272654; -0.962112>      *                
0111    <-0.643388; -0.76554>     <0.980081; 0.1986>     *                   

1000    <-0.939807; 0.341706>   <0.948218; 0.317621>     *                  
1001    <-0.412279; -0.911058>  <-0.244493; 0.969651>    *                  
1010    <0.272654; -0.962112>   <-0.81231; 0.583226>     *                  
1011    <0.980081; 0.1986>  <-0.643388; -0.76554>        *                  

1100    <-0.19796; 0.98021>   <-0.19796; 0.98021>      
1101    <-0.992438; -0.122744>  <-0.992438; -0.122744>
1110    <-0.679175; -0.733977>  <-0.679175; -0.733977>
1111    <0.680128; -0.733093>  <0.680128; -0.733093>


 * 
 */


/*


M1 

<0; 0>  <0; 0>  <1; 0>  <0; 0>
<0; 0>  <0; 0>  <0; 0>  <1; 0>
<1; 0>  <0; 0>  <0; 0>  <0; 0>
<0; 0>  <1; 0>  <0; 0>  <0; 0>

00  a    <0.532397; 0.846495>        <0.840344; 0.542053>       c
01  b    <-0.990555; 0.137115>       <-0.839716; 0.543026>      d
10  c   <0.840344; 0.542053>        <0.532397; 0.846495>        a 
11  d   <-0.839716; 0.543026>       <-0.990555; 0.137115>       b


M2 

<0; 0>  <1; 0>  <0; 0>  <0; 0>
<1; 0>  <0; 0>  <0; 0>  <0; 0>
<0; 0>  <0; 0>  <0; 0>  <1; 0>
<0; 0>  <0; 0>  <1; 0>  <0; 0>

00  a    <0.532397; 0.846495>  <-0.990555; 0.137115>    b
01  b   <-0.990555; 0.137115>   <0.532397; 0.846495>    a
10  c    <0.840344; 0.542053>  <-0.839716; 0.543026>    d
11  d   <-0.839716; 0.543026>   <0.840344; 0.542053>    c




*/
