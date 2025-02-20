namespace Lyt.Quantics.Engine.Gates.Ternary;

using MathNet.Numerics.LinearAlgebra;

public sealed class FredkinGate : ControlledGate
{
    // The Fredkin gate (also controlled-SWAP gate and conservative logic gate) is a
    // computational circuit suitable for reversible computing, invented by Edward Fredkin.
    // It is universal, which means that any logical or arithmetic operation can be constructed
    // entirely of Fredkin gates. 
    // The Fredkin gate is a circuit or device with three inputs and three outputs that transmits the
    // first bit unchanged and swaps the last two bits if, and only if, the first bit is 1. 

    private static readonly Matrix<Complex> FredkinMatrix;

    static FredkinGate()
    {
        //    { 1, 0, 0, 0 , 0, 0, 0, 0},
        //    { 0, 1, 0, 0 , 0, 0, 0, 0},
        //    { 0, 0, 1, 0 , 0, 0, 0, 0},
        //    { 0, 0, 0, 1 , 0, 0, 0, 0},
        //    { 0, 0, 0, 0 , 1, 0, 0, 0},
        //    { 0, 0, 0, 0 , 0, 0, 1, 0},
        //    { 0, 0, 0, 0 , 0, 1, 0, 0},
        //    { 0, 0, 0, 0 , 0, 0, 0, 1},

        FredkinMatrix = Matrix<Complex>.Build.Dense(8, 8, Complex.Zero);
        for (int i = 0; i < 5; i++)
        {
            FredkinMatrix.At(i, i, Complex.One);
        }

        FredkinMatrix.At(5, 6, Complex.One);
        FredkinMatrix.At(6, 5, Complex.One);
        FredkinMatrix.At(7, 7, Complex.One);
    }

    public FredkinGate() : base(new SwapGate()) { }

    public override int ControlQuBits => 1;

    public override int TargetQuBits => 2;

    public override Matrix<Complex> Matrix => FredkinGate.FredkinMatrix;

    public override string Description => "The Controlled SWAP Gate, aka Fredkin Gate";

    public override string Name => "Fredkin";

    public override string AlternateName => "CSWAP";

    public override string CaptionKey => "CSwap";

    public override GateCategory Category => GateCategory.TernaryControlled;
}
