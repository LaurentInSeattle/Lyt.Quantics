namespace Lyt.Quantics.Engine.Gates.Base;

using MathNet.Numerics.LinearAlgebra;

public sealed class PhaseGate : Gate
{
    // The phase gate P rotates the phase of the |1> state by a given angle lambda
    //      P|0> = |0> 
    //      P|1> = e ^^ (i * lambda)
    // Note there is a special case of P  with lambda = Pi / 4 , often just called the T gate.
    // Similarly: with lambda = Pi / 2, this is the S gate 
    //            with lambda = Pi , this is the Z gate 

    // Matrix: 
    // P(λ)= {  1  0​    }
    //       {  0  eiλ​  }
    // This is a diagonal gate.
    //
    // P ( λ = π )     = Z
    // P ( λ = π / 2 ) = S
    // P ( λ = π / 4 ) = T

    private readonly Matrix<Complex> matrix;
    private readonly string captionKey;

    public PhaseGate(double lambda = Math.PI / 2.0)
    {
        this.Lambda = lambda;
        
        double sinReal = Math.Sin(lambda);
        double cosReal = Math.Cos(lambda);
        Complex eIotaLambda = new Complex(cosReal, sinReal);
        this.matrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);
        this.matrix.At(0, 0, Complex.One);
        this.matrix.At(1, 1, eIotaLambda);

        string captionAngle = lambda.ToString("F2");
        captionAngle = captionAngle.Replace(".", "_");
        this.captionKey = string.Concat("P_", captionAngle);
    }

    public double Lambda { get; private set; }

    public override Matrix<Complex> Matrix => this.matrix;

    public override string Name => "Phase";

    public override string AlternateName => "Phase Gate";

    public override string CaptionKey => this.captionKey;
}
