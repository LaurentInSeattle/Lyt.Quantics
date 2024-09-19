namespace Lyt.Quantics.Engine.Core;

public sealed class QuBit
{
    private Complex[] tensor;

    public QuBit(int pureState = 0)
    {
        Complex[] quBase = pureState == 0 ? [Complex.One, Complex.Zero] : [Complex.Zero, Complex.One];
        this.tensor = quBase;
    }

    public Complex[] Tensor => this.tensor; 

    public bool IsCollapsed { get; private set; }

    public List<int> Measure()
    {
        this.IsCollapsed = true;
        int length = this.tensor.Length;
        int resultLength = MathUtilities.Log2(length);
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
        double sumProbabilities=0.0;
        for (int i = 0; i < length; ++i)
        {
            double probability = sumProbabilities + probabilities[i]; 
            if (sample < probability)
            {
                slot = i ; 
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
