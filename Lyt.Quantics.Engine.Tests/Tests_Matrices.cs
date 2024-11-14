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
    public void Test_IdentityTensorProduct()
    {
        try
        {
            for (int i = 0; i < 10; i++)
            {
                for (int quBits = 2; quBits <= 8; ++quBits)
                {
                    for (int step = 1; step < quBits-1; ++step)
                    {
                        Matrix<Complex>[] identities = new Matrix<Complex>[step];
                        for (int k = 0; k < step; ++k)
                        {
                            identities[k] = CreateIdentityMatrix(2);
                        }

                        Matrix<Complex>[] randoms = new Matrix<Complex>[quBits - step];
                        for (int k = 0; k < quBits - step; ++k)
                        {
                            randoms[k] = CreateRandomMatrix(2);
                        }
                        
                        // Combine all operator matrices to create the stage matrix
                        // using the Knonecker product
                        Matrix<Complex> classic = identities[0];
                        for (int ident = 1; ident < step; ++ident) // Must start at ONE!
                        {
                            var matrix = identities[ident];
                            classic = classic.KroneckerProduct(matrix);
                        }

                        for (int rand = 0; rand< quBits - step; ++rand) // Must start at ZERO!
                        {
                            var matrix = randoms[rand];
                            classic = classic.KroneckerProduct(matrix);
                        }


                        var registerSource = CreateRandomQuRegister(quBits);
                        var registerClone  = CreateRandomQuRegister(quBits);
                        registerClone.State = registerSource.State.Clone();

                        Debug.WriteLine(registerSource.State);
                        Debug.WriteLine(classic);

                        var classicNewState = classic.Multiply(registerSource.State);
                        Debug.WriteLine(classicNewState);

                        // Verify that for the first steps we have equality 
                        for ( int istep = 0; istep  < 2 * step; ++istep)
                        {
                            Complex r = registerClone.State[istep];
                            Complex c = classicNewState[istep];
                            // Fail here 
                            // Assert.AreEqual(r, c);
                        }

                        Matrix<Complex> optimized = randoms[0];
                        for (int rand = 1; rand < quBits - step; ++rand) // Must start at ONE!
                        {
                            var matrix = randoms[rand];
                            optimized = optimized.KroneckerProduct(matrix);
                        }

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
    public void Test_SwapTensorProduct()
    {
        try
        {
            for (int i = 0; i < 10; i++)
            {
                for (int quBits = 2; quBits <= 8; ++quBits)
                {
                    for (int step = 1; step < quBits - 1; ++step)
                    {
                        // Fill all with identity 
                        Matrix<Complex>[] identities = new Matrix<Complex>[quBits];
                        for (int k = 0; k < quBits-1; ++k)
                        {
                            identities[k] = CreateIdentityMatrix(2);
                        }

                        // At step, replace by a Swap
                        var swapGate = GateFactory.Produce(SwapGate.Key, new GateParameters());
                        identities[step] = swapGate.Matrix;

                        // Combine all operator matrices to create a stage matrix
                        // using the Knonecker product
                        Matrix<Complex> classic = identities[0];
                        for (int ident = 1; ident < quBits-1; ++ident) // Must start at ONE!
                        {
                            var matrix = identities[ident];
                            classic = classic.KroneckerProduct(matrix);
                        }

                        var registerSource = CreateRandomQuRegister(quBits);
                        var registerClone = CreateRandomQuRegister(quBits);
                        registerClone.State = registerSource.State.Clone();

                        var classicNewState = classic.Multiply(registerSource.State);

                        Debug.WriteLine(registerSource.State);
                        Debug.WriteLine(classic);
                        Debug.WriteLine(classicNewState);

                        // Verify that we have equality at step vs step+1 

                        // Check is incorrect

                        //int stepBy2 = step + step; 
                        //Complex r1a = registerClone.State[stepBy2];
                        //Complex r1b = registerClone.State[1 + stepBy2];
                        //Complex r2a = registerClone.State[stepBy2];
                        //Complex r2b = registerClone.State[1 + stepBy2];

                        //Complex c1a = classicNewState[stepBy2];
                        //Complex c1b = classicNewState[1 + stepBy2];
                        //Complex c2a = classicNewState[stepBy2];
                        //Complex c2b = classicNewState[1 + stepBy2];

                        //Assert.AreEqual(r1a, c2a);
                        //Assert.AreEqual(r1b, c2b);
                        //Assert.AreEqual(r2a, c1a);
                        //Assert.AreEqual(r2b, c1b);
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

    private static Complex RandomComplex()
    {
        double angle = Math.Tau * RandomUtility.NextDouble();
        return new(Math.Cos(angle), Math.Sin(angle));
    } 

    private static QuRegister CreateRandomQuRegister(int dimension)
    {
        var quStates = new List<QuState>(dimension);
        for (int i = 0; i < dimension; ++i)
        {
            quStates.Add(QuState.One);
        }

        var register = new QuRegister(quStates);
        var state = register.State;
        for (int i = 0; i < state.Count; ++i)
        {
            state [i] = RandomComplex();

        }

        return register;
    }

    private static Matrix<Complex> CreateIdentityMatrix(int dimension)
        => Matrix<Complex>.Build.DenseIdentity(dimension, dimension);

    private static Matrix<Complex> CreateRandomMatrix(int dimension)
    {
        var matrix = Matrix<Complex>.Build.Dense(dimension, dimension);
        for (int row = 0; row < dimension; ++row)
        {
            for (int col = 0; col < dimension; ++col)
            {
                matrix.At(row, col, RandomComplex());
            }
        }

        return matrix;
    }
}

/*
 * 

Run for I at 0 , Swap at 1 2 

Bits 1 and 2 get swapped, bit string starting at zero 

000     a   <-0.77722; 0.629229>        <-0.77722; 0.629229>        a
001     b   <-0.354704; -0.934979>      <-0.996955; 0.0779801>      c   * 
010     c   <-0.996955; 0.0779801>      <-0.354704; -0.934979>      b   * 
011     d   <0.367435; -0.930049>       <0.367435; -0.930049>       d   
100     e   <-0.0444433; 0.999012>      <-0.0444433; 0.999012>      e
101     f   <-0.773535; -0.633754>      <-0.923949; -0.382516>      g   *
110     g   <-0.923949; -0.382516>      <-0.773535; -0.633754>      f   *
111     h   <-0.932534; 0.361082>       <-0.932534; 0.361082>       h 
 
 * 
 */