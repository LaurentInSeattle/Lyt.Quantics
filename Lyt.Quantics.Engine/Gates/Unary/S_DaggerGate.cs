namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;

#pragma warning disable IDE1006 // Naming Styles
public sealed class S_DaggerGate : Gate
#pragma warning restore IDE1006 
{
    // Single-qubit rotation about the Z axis, congugate transpose 
    // This is a diagonal gate. 
    // Special case of the phase gate with phase angle equal to π / 2

    private static readonly Matrix<Complex> S_DaggerGateMatrix;

    static S_DaggerGate()
    {
        var t_matrix = new S_Gate().Matrix;
        S_DaggerGateMatrix = t_matrix.ConjugateTranspose();
    }

    public override bool IsMutable => true;

    public override Matrix<Complex> Matrix => S_DaggerGate.S_DaggerGateMatrix;

    public override string Description => "The S Dagger Gate.";

    public override string Name => "Phase Dagger";

    public override string AlternateName => "S Dagger";

    public override string CaptionKey => "Sdg";

    public override GateCategory Category => GateCategory.Phase;
}
