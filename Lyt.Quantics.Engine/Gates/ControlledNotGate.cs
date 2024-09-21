namespace Lyt.Quantics.Engine.Gates;

public sealed class ControlledNotGate : Gate
{
    private static readonly Complex[,] ControlledNotMatrix = new Complex[,]
    {
        { 1, 0, 0, 0 },
        { 0, 1, 0, 0 },
        { 0, 0, 0, 1 },
        { 0, 0, 1, 0 }
    };

    public override Complex[,] Matrix => ControlledNotGate.ControlledNotMatrix;

    public override string Name => "Controlled Not";

    public override string AlternateName => "CX";

    public override string Caption => "CNOT";
}
