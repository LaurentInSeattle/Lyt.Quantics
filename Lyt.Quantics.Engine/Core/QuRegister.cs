// #define VERBOSE 

namespace Lyt.Quantics.Engine.Core;

// Cannot use Vector<Complex> using System.Numerics 
// but we still need System.Numerics for Complex 
// Be careful when using global usings 
using MathNet.Numerics.LinearAlgebra;
using static MathUtilities;

/// <summary> Result of Combining QuBit's </summary>
public sealed partial class QuRegister
{
    // Preloaded swaps limited to 18 for now  
    public const int MaxQubits = 18;

    // Runs will be threaded above this limit
    public const int ThreadedRunAtQubits = 12;

    // Ui will launch a progress dialog above this limit
    public const int UiThreadedRunAtQubits = 16;

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

    public int QuBitCount => MathUtilities.IntegerLog2(this.state.Count);

    public double[] QuBitProbabilities { get; private set; } = [];

    public double[] KetProbabilities { get; private set; } = [];

    public Tuple<string, double, int>[] BitValuesProbabilities { get; private set; } = [];

    // For unit tests 
    public void Apply(Gate gate) => this.state = gate.Matrix.Multiply(this.state);

    public void Calculate()
    {
        this.QuBitProbabilities = new double[this.QuBitCount];
        this.KetProbabilities = new double[this.State.Count];

        this.CalculateKetProbabilities();
        this.CalculateBitValuesProbabilities();
        this.CalculateQuBitProbabilities();
    }

    public void CalculateKetProbabilities()
    {
        for (int i = 0; i < this.state.Count; ++i)
        {
            Complex complex = this.state[i];
            var conjugate = Complex.Conjugate(complex);
            this.KetProbabilities[i] = (conjugate * complex).Real;
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

    }

    public void CalculateQuBitProbabilities()
    {
        int stateCount = this.state.Count;
        int quBitCount = MathUtilities.IntegerLog2(stateCount);
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
                    probability += this.BitValuesProbabilities[k].Item2;
                }
            }

            this.QuBitProbabilities[quBit] = probability;
        }

#if VERBOSE
        Debug.WriteLine("");
        for (int i = 0; i < quBitCount; ++i)
        {
            Debug.Write(probabilities[i].ToString("F2") + "  ");
        }
        Debug.WriteLine("");
#endif // VERBOSE
    }

    public void CalculateBitValuesProbabilities()
    {
        int length = this.state.Count;
        int stringLength = MathUtilities.IntegerLog2(length);
        char[] charArray = new char[stringLength];
        var bitValuesProbabilities = new List<Tuple<string, double, int>>(length);
        for (int i = 0; i < length; ++i)
        {
            int k = i;
            for (int j = 0; j < stringLength; ++j)
            {
                charArray[j] = (k & 1) == 0 ? '0' : '1';
                k >>= 1;
            }

            int rank = 0;
            int multiplier = 1;
            for (int j = stringLength - 1; j >= 0; --j)
            {
                int bitValue = charArray[j] == '0' ? 0 : 1;
                rank += bitValue * multiplier;
                multiplier *= 2;
            }

            string bits = new(charArray);
            bitValuesProbabilities.Add(new Tuple<string, double, int>(bits, this.KetProbabilities[i], rank));
        }

        var sortedBitValuesProbabilities = new Tuple<string, double, int>[length];
        for (int i = 0; i < length; ++i)
        {
            var kvp = bitValuesProbabilities[i];
            sortedBitValuesProbabilities[kvp.Item3] = kvp;
        }

#if VERBOSE
        Debug.WriteLine("");
        for (int i = 0; i < length; ++i)
        {
            var bvp = bitValuesProbabilities[i];
            Debug.Write(" " + bvp.Item1 + ":  " + bvp.Item2.ToString("F2") + "\t");
        }
        Debug.WriteLine("");
#endif // VERBOSE
        this.BitValuesProbabilities = sortedBitValuesProbabilities;
    }

    public List<int> Measure()
    {
        int length = this.state.Count;
        double sample = RandomUtility.NextDouble();
        int slot = -1;
        double sumProbabilities = 0.0;
        for (int i = 0; i < length; ++i)
        {
            sumProbabilities += this.KetProbabilities[i];
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
