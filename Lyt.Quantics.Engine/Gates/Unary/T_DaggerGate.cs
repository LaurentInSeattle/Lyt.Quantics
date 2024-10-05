﻿//  T_DaggerGate

namespace Lyt.Quantics.Engine.Gates.Unary;

using static MathUtilities;

using MathNet.Numerics.LinearAlgebra;

#pragma warning disable IDE1006 // Naming Styles
public sealed class T_DaggerGate : Gate
#pragma warning restore IDE1006 
{
    // Single qubit T dagger gate .
    // Equivalent to a minus π/4 radian rotation about the Z axis.
    // Conjugate transpose of the T_Gate 

    private static readonly Matrix<Complex> T_DaggerGateMatrix;

    static T_DaggerGate()
    {
        var t_matrix = new T_Gate().Matrix;
        T_DaggerGateMatrix = t_matrix.ConjugateTranspose(); 
    }

    public override Matrix<Complex> Matrix => T_DaggerGate.T_DaggerGateMatrix;

    public override string Name => "T Dagger";

    public override string AlternateName => "Minus Pi Over 8";

    public override string CaptionKey => "Tdg";
}