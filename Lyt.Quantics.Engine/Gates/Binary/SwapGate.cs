﻿namespace Lyt.Quantics.Engine.Gates.Binary;

using MathNet.Numerics.LinearAlgebra;

public sealed class SwapGate : Gate
{
    // The swap gate swaps two qubits. 
    public const string Key = "Swap";

    public static readonly Matrix<Complex> SwapMatrix;

    static SwapGate()
    {
        //    { 1, 0, 0, 0 },
        //    { 0, 0, 1, 0 },
        //    { 0, 1, 0, 0 },
        //    { 0, 0, 0, 1 },
        SwapMatrix = Matrix<Complex>.Build.Dense(4, 4, Complex.Zero);
        SwapMatrix.At(0, 0, Complex.One);
        SwapMatrix.At(3, 3, Complex.One);
        SwapMatrix.At(1, 2, Complex.One);
        SwapMatrix.At(2, 1, Complex.One);
    }

    public override Matrix<Complex> Matrix => SwapGate.SwapMatrix;

    // For  SWAP the two Qubits are equivalent and hence considered both as Targets 
    public override int ControlQuBits => 0;

    public override int TargetQuBits => 2;

    public override string Description => "The Swap Gate.";

    public override string Name => "Swap";

    public override string AlternateName => "Swap";

    public override string CaptionKey => SwapGate.Key;

    /// <summary> In this category for UI layout but is NOT controlled </summary>
    public override GateCategory Category => GateCategory.BinaryControlled;
}
