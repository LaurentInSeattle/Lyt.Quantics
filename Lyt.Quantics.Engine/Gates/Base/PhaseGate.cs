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

    public PhaseGate(double theta)
        : this(theta, isPiDivisor: false)
        => this.ParameterCaption = theta.ToString("F2");

    public PhaseGate(int piDivisor, bool isPositive)
        : this((isPositive ? 1.0 : -1.0) * Math.PI / piDivisor, isPiDivisor: true)
        => this.ParameterCaption = string.Format("{0}π/{1}", (isPositive ? "+" : "-"), piDivisor);

    private PhaseGate(double lambda, bool isPiDivisor)
    {
        this.Lambda = lambda;
        this.IsPiDivisor = isPiDivisor;

        double sinReal = Math.Sin(lambda);
        double cosReal = Math.Cos(lambda);
        Complex eIotaLambda = new(cosReal, sinReal);
        this.matrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);
        this.matrix.At(0, 0, Complex.One);
        this.matrix.At(1, 1, eIotaLambda);
    }

    public double Lambda { get; private set; }

    public override Matrix<Complex> Matrix => this.matrix;

    public override string Name => "Phase";

    public override string AlternateName => "Phase Gate";

    public override string CaptionKey => "Ph";

    public bool IsPiDivisor { get; private set; } = true;

    public int PiDivisor { get; private set; } = 2;

    public bool IsPositive { get; private set; } = true;

    public override string ParameterCaption { get; set; } = string.Empty;

    public override bool IsParametrized => true;

    public override GateCategory Category => GateCategory.H_Phase;
}
