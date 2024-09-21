using Lyt.Quantics.Engine.Gates;

namespace Lyt.Quantics.Engine.Core;

public sealed class QuBit : IEquatable<QuBit>
{
    // Cannot use Vector<Complex> !!! See TestVectors below 
    private Complex[] tensor;

    public QuBit(BasisState basisState) => this.tensor = basisState.ToTensor();

    public Complex[] Tensor => this.tensor;

    public bool IsCollapsed { get; private set; }

    public int CollapsedValue { get; private set; }

    public int Measure()
    {
        Complex complex = this.tensor[0];
        Complex conjugate = Complex.Conjugate(complex);
        double probability = (conjugate * complex).Real;
        double sample = RandomUtility.NextDouble();
        int result = 0;
        if (sample > probability)
        {
            result = 1;
        }

        Debug.WriteLine(result);

        this.IsCollapsed = true;
        this.CollapsedValue = result;
        return result;
    }

    public void Apply(UnaryGate gate) => this.tensor = MathUtilities.Transform(this.tensor, gate.Matrix);

    public bool Equals(QuBit? other)
    {
        if ((other is null) || ( this.tensor.Length != other.Tensor.Length))
        {
            return false;
        }

        for (int i = 0; i < this.tensor.Length; ++i)
        {
            if( this.tensor[i] != other.tensor[i]) 
            { 
                return false; 
            }
        }

        return true;
    } 

    //public static void TestVectors()
    //{
    //    // This compiles but crashes at run time 
    //    //var zero = new Vector<Complex>(new QuBit(BasisState.Zero).Tensor);
    //    //var one = new Vector<Complex>(new QuBit(BasisState.One).Tensor);
    //    //var x = zero + one;
    //    //Debug.WriteLine(x);
    //}
}
