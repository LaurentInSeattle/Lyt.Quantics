namespace Lyt.Quantics.Engine.Gates.Ternary;

using MathNet.Numerics.LinearAlgebra;

public sealed class ToffoliGate : ControlledGate
{
    // the Toffoli gate, also known as the CCNOT gate (“controlled-controlled-not”),
    // invented by Tommaso Toffoli, is a CNOT gate with two control qubits and one target
    // qubit. That is, the target qubit (third qubit) will be inverted if the first and
    // second qubits are both 1. It is a universal reversible logic gate, which means
    // that any classical reversible circuit can be constructed from Toffoli gates. 
    //
    // Note: The Toffoli gate also works like a 'AND' gate.

    private static readonly Matrix<Complex> ToffoliMatrix;

    static ToffoliGate()
    {
        //    { 1, 0, 0, 0 , 0, 0, 0, 0},
        //    { 0, 1, 0, 0 , 0, 0, 0, 0},
        //    { 0, 0, 1, 0 , 0, 0, 0, 0},
        //    { 0, 0, 0, 1 , 0, 0, 0, 0},
        //    { 0, 0, 0, 0 , 1, 0, 0, 0},
        //    { 0, 0, 0, 0 , 0, 1, 0, 0},
        //    { 0, 0, 0, 0 , 0, 0, 0, 1},
        //    { 0, 0, 0, 0 , 0, 0, 1, 0},

        ToffoliMatrix = Matrix<Complex>.Build.Sparse(8, 8, Complex.Zero);
        for (int i = 0; i < 6; i++)
        {
            ToffoliMatrix.At(i, i, Complex.One);
        }

        ToffoliMatrix.At(6, 7, Complex.One);
        ToffoliMatrix.At(7, 6, Complex.One);
    }

    public ToffoliGate() : base(new ControlledNotGate()) { }

    public override Matrix<Complex> Matrix => ToffoliGate.ToffoliMatrix;

    public override int ControlQuBits => 2;

    public override int TargetQuBits => 1;

    public override string Description => "The Controlled Controlled NOT Gate, aka Toffoli Gate";

    public override string Name => "Toffoli";

    public override string AlternateName => "CCNOT";

    public override string CaptionKey => "CCX";

    public override GateCategory Category => GateCategory.TernaryControlled;
}
