namespace Lyt.Quantics.Engine.Gates.Unary;

using MathNet.Numerics.LinearAlgebra;
using System.Numerics;

public sealed class SX_Gate : Gate
{
    // Single qubit SX gate 
    // The SX Gate acts on a single Qubit. The SX Gate performs the rotation about the X-axis
    // of the Bloch Sphere by 90° or π/2 radians in the counter-clockwise direction.
    //
    // Note – The SX Gate is a special case of RX Gate where the parameter(the phase change) is
    // 90° or π/2 radians.
    //  
    // Note – The SX Gate is sometimes also called √X Gate.This is because applying the SX Gate
    // twice produces the same effect as a X Gate.

    private static readonly Matrix<Complex> sX_GateMatrix;

    static SX_Gate()
    {
        // 1/2 *  { 1 + i , 1 - i },
        //        { 1 - i , 1 + i } 
        var OnePlusIota = new Complex(1, 1) / 2.0;
        var OneMinusIota = new Complex(1, -1) / 2.0;
        sX_GateMatrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);
        sX_GateMatrix.At(0, 0, OnePlusIota);
        sX_GateMatrix.At(0, 1, OneMinusIota);
        sX_GateMatrix.At(1, 0, OneMinusIota);
        sX_GateMatrix.At(1, 1, OnePlusIota);
    }

    public override Matrix<Complex> Matrix => SX_Gate.sX_GateMatrix;

    public override string Name => "SX Gate";

    public override string AlternateName => "SQRT of X";

    public override string CaptionKey => "SX";
}