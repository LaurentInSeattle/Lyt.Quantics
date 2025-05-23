﻿namespace Lyt.Quantics.Engine.Gates.Binary;

using MathNet.Numerics.LinearAlgebra;

public sealed class ControlledNotGate : ControlledGate
{
    // The CNOT (or controlled Pauli-X) gate can be described as the gate that maps the
    // basis states:  |a,b> => |a, a xor b> 
    public new const string Key = "CX";

    private static readonly Matrix<Complex> ControlledNotMatrix;

    static ControlledNotGate()
    {
        ControlledNotMatrix = Matrix<Complex>.Build.Dense(4, 4, Complex.Zero);
        //    { 1, 0, 0, 0 },
        //    { 0, 1, 0, 0 },
        //    { 0, 0, 0, 1 },
        //    { 0, 0, 1, 0 }
        ControlledNotMatrix.At(0, 0, Complex.One);
        ControlledNotMatrix.At(1, 1, Complex.One);
        ControlledNotMatrix.At(2, 3, Complex.One);
        ControlledNotMatrix.At(3, 2, Complex.One);
    }

    public ControlledNotGate() : base(new PauliXGate()) { }

    public override Matrix<Complex> Matrix => ControlledNotGate.ControlledNotMatrix;

    public override string Description => "The Controlled Not Gate, CNOT.";

    public override int ControlQuBits => 1;

    public override int TargetQuBits => 1;

    public override string Name => "Controlled Not";

    public override string AlternateName => "CNOT";

    public override string CaptionKey { get; set; } = ControlledNotGate.Key;

    public override GateCategory Category => GateCategory.BinaryControlled;
}
