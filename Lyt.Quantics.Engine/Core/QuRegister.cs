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
        for (int i = 0; i < initialStates.Count; ++i)
        {
            QuState quState = initialStates[i]; 
            Complex[] quBitState = quState.ToTensor();
            this.state.At(i, quBitState[0]);
            this.state.At(i+1, quBitState[1]);
        }
    }

    public Vector<Complex> State 
    {  
        get  => this.state; 
        set => this.state = value; 
    }

    public QuRegister(QuBit quBit1, QuBit quBit2)
        => this.state = MathUtilities.TensorProduct(quBit1.State, quBit2.State);

    public void Apply(Gate gate) => this.state = gate.Matrix.Multiply(this.state);

    public List<int> Measure()
    {
        int length = this.state.Count;
        int resultLength = MathUtilities.IntegerLog2(length);
        double[] probabilities = new double[length];
        for (int i = 0; i < length; ++i)
        {
            Complex complex = this.state[i];
            var conjugate = Complex.Conjugate(complex);
            probabilities[i] = (conjugate * complex).Real;
        }

        List<int> result = new(resultLength);
        double sample = RandomUtility.NextDouble();
        int slot = -1;
        double sumProbabilities = 0.0;
        for (int i = 0; i < length; ++i)
        {
            double probability = sumProbabilities + probabilities[i];
            if (sample < probability)
            {
                slot = i;
                break;
            }
        }

        if (slot == -1)
        {
            throw new Exception("Failed to find slot ???");
        }

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
