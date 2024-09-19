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
}
