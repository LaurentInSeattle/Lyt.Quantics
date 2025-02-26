﻿namespace Lyt.Quantics.Engine.Gates.Binary;

using MathNet.Numerics.LinearAlgebra;

public sealed class FlippedControlledNotGate : Gate
{
    public const string Key = "FCX";

    private static readonly Matrix<Complex> FlippedControlledNotMatrix;

    static FlippedControlledNotGate()
    {
        //    { 1, 0, 0, 0 },
        //    { 0, 0, 0, 1 },
        //    { 0, 0, 1, 0 },
        //    { 0, 1, 0, 0 },
        FlippedControlledNotMatrix = Matrix<Complex>.Build.Dense(4, 4, Complex.Zero);
        FlippedControlledNotMatrix.At(0, 0, Complex.One);
        FlippedControlledNotMatrix.At(2, 2, Complex.One);
        FlippedControlledNotMatrix.At(1, 3, Complex.One);
        FlippedControlledNotMatrix.At(3, 1, Complex.One);
    }

    public override Matrix<Complex> Matrix => FlippedControlledNotGate.FlippedControlledNotMatrix;

    public override int ControlQuBits => 1;

    public override int TargetQuBits => 1;

    public override string Description => "The 'Flipped' Controlled NOT Gate.";

    public override string Name => "Flipped Controlled Not";

    public override string AlternateName => "FCNOT";

    public override string CaptionKey => FlippedControlledNotGate.Key;

    public override GateCategory Category => GateCategory.BinaryControlled;
}
