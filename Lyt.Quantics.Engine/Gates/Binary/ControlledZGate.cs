namespace Lyt.Quantics.Engine.Gates.Binary;

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

    // Like SWAP the two Qubits are equivalent and hence considered both as Targets 
    // https://quantumcomputing.stackexchange.com/questions/17677/does-control-and-target-matter-in-the-cz-controlled-z-gate

    public override int ControlQuBits => 0;

    public override int TargetQuBits => 2;

    public override string Name => "Controlled Z";

    public override string AlternateName => "Controlled Phase Flip";

    public override string CaptionKey => "CZ";

    public override GateCategory Category => GateCategory.E_BinaryControlled;
}
