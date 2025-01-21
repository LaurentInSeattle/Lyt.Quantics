// #define VERBOSE 

namespace Lyt.Quantics.Engine.Core;

// Cannot use Vector<Complex> using System.Numerics 
// but we still need System.Numerics for Complex 
// Be careful when using global usings 
using MathNet.Numerics.LinearAlgebra;
using static MathUtilities;

/// <summary> Result of Combining QuBit's </summary>
public sealed class QuRegister
{
    public const int MaxQubits = 10; // For now ~ 10, more could be doable ? 

    private Vector<Complex> state;

    public QuRegister(Vector<Complex> state) => this.state = state;

    // For unit tests 
    public QuRegister(QuBit quBit1, QuBit quBit2)
        => this.state = MathUtilities.TensorProduct(quBit1.State, quBit2.State);

    public QuRegister(List<QuState> initialStates)
    {
        int count = initialStates.Count;
        if ((count <= 0) || (count > MaxQubits))
        {
            throw new ArgumentException("Invalid qubit count");
        }

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

    public QuRegister(int quBits)
    {
        if ((quBits <= 0) || (quBits > MaxQubits))
        {
            throw new ArgumentException("Invalid qubit count");
        }

        this.state = Vector<Complex>.Build.Dense(2 * quBits);
        List<Vector<Complex>> vectors = new(quBits);
        for (int i = 0; i < quBits; ++i)
        {
            Complex[] rand = [MathUtilities.RandomUnitComplex(), MathUtilities.RandomUnitComplex()];
            vectors.Add(rand.ToVector());
        }

        this.state = vectors[0];
        for (int i = 1; i < quBits; ++i) // start at One 
        {
            this.state = MathUtilities.TensorProduct(this.state, vectors[i]);
        }
    }

    public void Randomize()
    {
        for (int i = 0; i < this.state.Count; ++i)
        {
            state[i] = MathUtilities.RandomUnitComplex();
        }
    }

    public QuRegister DeepClone() => new(this.State.Clone());

    public Vector<Complex> State
    {
        get => this.state;
        set => this.state = value;
    }

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

    public void ApplyUnaryGateAtPosition(Gate gate, KetMap ketMap, int position )
    {
        if (!gate.IsUnary)
        {
            throw new Exception("Gate should be unary.");
        }

        int stateCount = this.state.Count;
        int quBitCount = MathUtilities.IntegerLog2(stateCount);
        if ((position < 0) || (position >= quBitCount))
        {
            throw new ArgumentException("Invalid position");
        }

        this.Swap(ketMap, 0, position);
        this.ApplyUnaryGateOnQuBitZero(gate);
        this.Swap(ketMap, 0, position);
    }

    public void ApplyUnaryGateOnQuBitZero(Gate gate)
    {
        if ( ! gate.IsUnary)
        {
            throw new Exception("Gate should be unary.");
        }

        var matrix = gate.Matrix;
        Vector<Complex> subState = Vector<Complex>.Build.Dense(2);
        int half = this.State.Count / 2; 
        for (int k = 0; k < half; ++k)
        {
            subState.At(0, this.State[k]);
            subState.At(1, this.State[k+half]);
            subState = matrix.Multiply(subState);
            this.state[k] = subState.At(0);
            this.state[k+half] = subState.At(1);
        }
    }

    /// <summary> This should perform just like applying a binary swap gate on qubits indices i and j </summary>
    /// <remarks> Assumes that i < j </remarks>
    public void Swap(KetMap ketMap, int i, int j)
    {
        KetMap reducedKetMap = ketMap.Reduce(i, j);
        List<Tuple<int, int>> swaps = [];
        for (int k1 = 0; k1 < this.State.Count; ++k1)
        {
            // for the state at k index: 
            // if bit #i is not equal to bit #j, we need to swap values 
            // The condition is equivalent to: 
            // if bit #i is set an bit #j is not set 
            // OR 
            // if bit #j is set an bit #i is not set 
            bool areDifferent = ketMap.Get(k1, i) ^ ketMap.Get(k1, j);
            if (areDifferent)
            {
                // This index k1 needs to be swapped with another one, k2, so... Find k2 
                int k2 = ketMap.SwapMatch(reducedKetMap, k1, i, j);
                bool alreadyThere = false;
                foreach (var tuple in swaps)
                {
                    if ((tuple.Item1 == k2) && (tuple.Item2 == k1))
                    {
                        alreadyThere = true;
                        break;
                    }
                }

                if (!alreadyThere)
                {
                    swaps.Add(new(k1, k2));
                }
            }
        }

        foreach (var swap in swaps)
        {
            int i1 = swap.Item1;
            int i2 = swap.Item2;

#if VERBOSE            
            Debug.WriteLine(i1 + " <-> " + i2);
#endif // VERBOSE            

            Complex state1 = this.State[i1];
            Complex state2 = this.State[i2];
            this.State[i1] = state2;
            this.State[i2] = state1;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new(this.State.Count * 8);
        sb.AppendLine("");
        for (int i = 0; i < this.State.Count; ++i)
        {
            Complex x = this.State[i];
            sb.Append(x.ToString());
            if ((i & 1) != 0)
            {
                sb.AppendLine("");
            }
        }

        return sb.ToString();
    }

    public bool IsAlmostEqualTo(QuRegister other)
        => this.state.IsAlmostEqualTo(other.state);
}
