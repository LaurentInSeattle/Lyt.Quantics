﻿namespace Lyt.Quantics.Engine.Gates.Binary;

using MathNet.Numerics.LinearAlgebra;

public sealed class AntiControlledNotGate : Gate
{
    // The CNOT (or controlled Pauli-X) gate can be described as the gate that maps the
    // basis states:  |a,b> => |a, a xor b> 

    private static readonly Matrix<Complex> AntiControlledNotMatrix; 

    static AntiControlledNotGate()
    {
        AntiControlledNotMatrix = Matrix<Complex>.Build.Sparse(4, 4, Complex.Zero);
        //    { 0, 1, 0, 0 },
        //    { 1, 0, 0, 0 },
        //    { 0, 0, 1, 0 },
        //    { 0, 0, 0, 1 }
        AntiControlledNotMatrix.At(0, 1, Complex.One);
        AntiControlledNotMatrix.At(1, 0, Complex.One);
        AntiControlledNotMatrix.At(2, 2, Complex.One);
        AntiControlledNotMatrix.At(3, 3, Complex.One);
    }

    public override Matrix<Complex> Matrix => AntiControlledNotGate.AntiControlledNotMatrix;

    public override string Name => "Anti-Controlled Not";

    public override string AlternateName => "ACNOT";

    public override string CaptionKey => "ACX";

    public override GateCategory Category => GateCategory.D_BinaryControlled;
}