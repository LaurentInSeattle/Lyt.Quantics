namespace Lyt.Quantics.Engine.Gates;

using static MathUtilities;
using static Math;

using MathNet.Numerics.LinearAlgebra;

public sealed class PhaseRotationGate : Gate
{
    // The phase gate R rotates the phase of the |1> state by a given angle phi
    //      R|0> = |0> 
    //      R|1> = e ^^ (i * phi)
    // Note that Z is a special case of R  with phi = Pi / 4 , often just called the T gate.

    private readonly Matrix<Complex> phaseRotationGateMatrix;

    public PhaseRotationGate(double phi)
    {
        //      { 1, 0 },
        //      { 0, new Complex ( Cos(phi), Sin(phi) )}
        var complex = new Complex(Cos(phi), Sin(phi));
        this.phaseRotationGateMatrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);
        this.phaseRotationGateMatrix.At(0, 0, Complex.One);
        this.phaseRotationGateMatrix.At(1, 1, complex);

    }

    public override Matrix<Complex> Matrix => this.phaseRotationGateMatrix;

    public override string Name => "Phase Rotation Gate";

    public override string AlternateName => "Rotation Gate";

    public override string CaptionKey => "R";
}
