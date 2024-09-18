using Lyt.Quantics.Engine.Utilities;

namespace Lyt.QuantumEngine.Core;

public sealed class QuBit
{
    private Complex[] tensor;

    public QuBit(int pureState = 0)
    {
        Complex[] quBase = pureState == 0 ? [Complex.One, Complex.Zero] : [Complex.Zero, Complex.One]; 
        this.tensor = quBase;
    }

    public static void Combine(QuBit quBit1, QuBit quBit2)
    {
        var newTensor = MathUtilities.TensorProduct(quBit1.tensor, quBit2.tensor);
        quBit1.tensor = newTensor;
        quBit2.tensor = newTensor;
    }

    public int Measure()
    {
        // For now 
        return 0; 
    }
}
