namespace Lyt.Quantics.Engine.Gates;

using static MathUtilities;

public sealed class PhaseRotationGate : UnaryGate
{
    // The phase gate R rotates the phase of the |1> state by a given angle phi
    //      R|0> = |0> 
    //      R|1> = e ^^ (i * phi)
    // Note that Z is a special case of R  with phi = Pi / 4 , often just called the T gate.

    public PhaseRotationGate(double phi)
    {
        // TODO 
    }

    private readonly Complex[,] PhaseRotationGateMatrix = new Complex[,]
    {
        // TODO 
        { 1, 0 },
        { 0, new Complex ( SqrtOfTwo / 2.0 , SqrtOfTwo / 2.0 )}
    };

    public override Complex[,] Matrix => this.PhaseRotationGateMatrix;

    public override string Name => "Phase Rotation Gate";

    public override string AlternateName => "Rotation Gate";

    public override string Caption => "R";
}
