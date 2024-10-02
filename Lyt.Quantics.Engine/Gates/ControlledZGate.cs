namespace Lyt.Quantics.Engine.Gates;

using MathNet.Numerics.LinearAlgebra;

public sealed class ControlledZGate : Gate
{
    // The the CZ gate applies a phase flip (change in the relative phase) to
    // the target qubit only when the control qubit is in the state |1⟩.
    // If the control qubit is in the state |0⟩, the CZ gate does not affect the
    // target qubit. 

    private static readonly Matrix<Complex> ControlledZMatrix; 

    static ControlledZGate()
    {
        ControlledZMatrix = Matrix<Complex>.Build.Sparse(4, 4, Complex.Zero);
        //    { 1, 0, 0,  0 },
        //    { 0, 1, 0,  0 },
        //    { 0, 0, 1,  0 },
        //    { 0, 0, 0, -1 }
        ControlledZMatrix.At(0, 0, Complex.One);
        ControlledZMatrix.At(1, 1, Complex.One);
        ControlledZMatrix.At(2, 2, Complex.One);
        ControlledZMatrix.At(3, 3, -Complex.One);
    }

    public override Matrix<Complex> Matrix => ControlledZGate.ControlledZMatrix;

    public override string Name => "Controlled Z";

    public override string AlternateName => "Controlled Phase Flip";

    public override string CaptionKey => "CZ";
}
