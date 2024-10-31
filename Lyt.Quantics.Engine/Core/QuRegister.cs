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
        List<Vector<Complex>> vectors = new(initialStates.Count);
        for (int i = 0; i < initialStates.Count; ++i)
        {
            vectors.Add(initialStates[i].ToVector());
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

    public List<double> KetProbabilities()
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
            string ket =
                "|" + BinaryStringUtilities.ToBinary(i, MathUtilities.IntegerLog2(length)) + ">";
            Debug.Write(ket + "  " + probabilities[i].ToString("F2") + "\t");
        }
        Debug.WriteLine("");
#endif // VERBOSE

        return probabilities;
    }

    public List<double> QuBitProbabilities()
    {
        int stateCount = this.state.Count;
        int quBitCount = MathUtilities.IntegerLog2(stateCount);
        var bitValuesProbabilities = this.BitValuesProbabilities();
        List<double> probabilities = new(quBitCount);
        for (int quBit = 0; quBit < quBitCount; ++quBit)
        {
            double probability = 0.0;
            for (int k = 0; k < stateCount; ++k)
            {
                if (MathUtilities.IsBitSet(k, quBit))
                {
#if VERBOSE
                    Debug.WriteLine(
                        "Qubit: " + quBit + " at " + k + " adding: " + bitValuesProbabilities[k].Item2.ToString("F2"));
#endif // VERBOSE
                    probability += bitValuesProbabilities[k].Item2;
                }
            }

            probabilities.Add(probability);
        }

#if VERBOSE
        Debug.WriteLine("");
        for (int i = 0; i < quBitCount; ++i)
        {
            Debug.Write(probabilities[i].ToString("F2") + "  ");
        }
        Debug.WriteLine("");
#endif // VERBOSE

        return probabilities;
    }

    public List<Tuple<string, double>> BitValuesProbabilities()
    {
        int length = this.state.Count;
        var bitValuesProbabilities = new List<Tuple<string, double>>(length);
        List<double> probabilities = this.KetProbabilities();
        for (int i = 0; i < length; ++i)
        {
            string ket = BinaryStringUtilities.ToBinary(i, MathUtilities.IntegerLog2(length));
            string bits = ket.Reverse();
            bitValuesProbabilities.Add(new Tuple<string, double>(bits, probabilities[i]));
        }

        bitValuesProbabilities =
            [.. (from bvp in bitValuesProbabilities orderby bvp.Item1 select bvp)];
#if VERBOSE
        Debug.WriteLine("");
        for (int i = 0; i < length; ++i)
        {
            var bvp = bitValuesProbabilities[i];
            Debug.Write(" " + bvp.Item1 + ":  " + bvp.Item2.ToString("F2") + "\t");
        }
        Debug.WriteLine("");
#endif // VERBOSE
        return bitValuesProbabilities;
    }

    public List<int> Measure()
    {
        List<double> probabilities = this.KetProbabilities();
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
