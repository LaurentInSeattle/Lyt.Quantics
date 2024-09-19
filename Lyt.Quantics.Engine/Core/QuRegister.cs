namespace Lyt.Quantics.Engine.Core;

/// <summary> Result of Combining QuBit's </summary>
public sealed class QuRegister
{
    private readonly List<QuBit> sourceQuBits; 
    private Complex[] tensor;

    public QuRegister(QuBit quBit1, QuBit quBit2)
    {
        this.sourceQuBits = [quBit1, quBit2]; 
        this.tensor = MathUtilities.TensorProduct(quBit1.Tensor, quBit2.Tensor);
    }

    public List<int> Measure()
    {
        int length = this.tensor.Length;
        int resultLength = MathUtilities.IntegerLog2(length);
        double[] probabilities = new double[length];
        for (int i = 0; i < length; ++i)
        {
            Complex complex = this.tensor[i];
            Complex conjugate = Complex.Conjugate(complex);
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

        for (int i = 0; i < resultLength; ++i)
        {
            Debug.WriteLine(result[i]);
        }

        return result;
    }
}
