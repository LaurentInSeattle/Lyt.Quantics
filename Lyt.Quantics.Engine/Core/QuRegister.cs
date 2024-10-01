// #define VERBOSE 

namespace Lyt.Quantics.Engine.Core;

// Cannot use Vector<Complex> using System.Numerics 
// but we still need System.Numerics for Complex 
// Be careful when using global usings 
using MathNet.Numerics.LinearAlgebra;

/// <summary> Result of Combining QuBit's </summary>
public sealed class QuRegister
{
    private Vector<Complex> state;

    public QuRegister(int quBitsCount) =>
        this.state = Vector<Complex>.Build.Dense(2 * quBitsCount);

    public QuRegister(List<QuState> initialStates)
    {
        this.state = Vector<Complex>.Build.Dense(2 * initialStates.Count);
        //for (int i = 0; i < initialStates.Count; ++i)
        //{
        //    QuState quState = initialStates[i];
        //    int j = i << 1; 
        //    Complex[] quBitState = quState.ToTensor();
        //    this.state.At(j, quBitState[0]);
        //    this.state.At(j + 1, quBitState[1]);
        //}

        List<Vector<Complex>> vectors = new(initialStates.Count);
        for (int i = 0; i < initialStates.Count; ++i)
        {
            vectors.Add (initialStates[i].ToVector());
        }

        this.state = vectors[0];
        for (int i = 1; i < initialStates.Count; ++i) // start at One 
        {
            this.state = MathUtilities.TensorProduct(this.state, vectors[i]);
        }
    }

    public Vector<Complex> State
    {
        get => this.state;
        set => this.state = value;
    }

    // For unit tests 
    public QuRegister(QuBit quBit1, QuBit quBit2)
        => this.state = MathUtilities.TensorProduct(quBit1.State, quBit2.State);

    // For unit tests 
    public void Apply(Gate gate) => this.state = gate.Matrix.Multiply(this.state);

    public List<double> Probabilities()
    {
        int length = this.state.Count;
        List<double> probabilities = new(length);
        for (int i = 0; i < length; ++i)
        {
            Complex complex = this.state[i];
            var conjugate = Complex.Conjugate(complex);
            probabilities.Add((conjugate * complex).Real);
        }

#if VERBOSE
        Debug.WriteLine("");
        for (int i = 0; i < length; ++i)
        {
            Debug.Write(probabilities[i] + "\t");
        }
        Debug.WriteLine("");
#endif // VERBOSE

        return probabilities;
    }

    public List<int> Measure()
    {
        List<double> probabilities = this.Probabilities();
        int length = this.state.Count;
        double sample = RandomUtility.NextDouble();
        int slot = -1;
        double sumProbabilities = 0.0;
        for (int i = 0; i < length; ++i)
        {
            sumProbabilities += probabilities[i];
            if (sample <= sumProbabilities)
            {
                slot = i;
                break;
            }
        }

        if (slot == -1)
        {
            throw new Exception("Failed to find slot ???");
        }

        int resultLength = MathUtilities.IntegerLog2(length);
        List<int> result = new(resultLength);
        for (int i = 0; i < resultLength; ++i)
        {
            int mask = 1 << i;
            int bitValue = ((slot & mask) != 0) ? 1 : 0;
            result.Insert(0, bitValue);
        }

#if VERBOSE
        for (int i = 0; i < resultLength; ++i)
        {
            Debug.WriteLine(result[i]);
        }
#endif // VERBOSE

        return result;
    }
}
