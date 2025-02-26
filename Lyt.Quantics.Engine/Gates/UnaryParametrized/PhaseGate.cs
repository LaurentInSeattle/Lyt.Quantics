﻿namespace Lyt.Quantics.Engine.Gates.UnaryParametrized;

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

    public PhaseGate(GateParameters parameters)
    {
        this.Angle = parameters.Angle;
        this.IsPiDivisor = parameters.IsPiDivisor;
        this.PiDivisor = parameters.PiDivisor;
        this.IsPositive = parameters.IsPositive;
        if (this.IsPiDivisor)
        {
            this.Angle = (this.IsPositive ? 1.0 : -1.0) * Math.PI / this.PiDivisor;
            this.AngleParameterCaption =
                this.PiDivisor == 1 ?
                    string.Format("{0}π", this.IsPositive ? "+" : "-") :
                    string.Format("{0}π/{1}", this.IsPositive ? "+" : "-", this.PiDivisor);
        }
        else
        {
            this.AngleParameterCaption = this.Angle.ToString("F2");
        }

        double sinReal = Math.Sin(this.Angle);
        double cosReal = Math.Cos(this.Angle);
        Complex eIotaLambda = new(cosReal, sinReal);
        this.matrix = Matrix<Complex>.Build.Dense(2, 2, Complex.Zero);
        this.matrix.At(0, 0, Complex.One);
        this.matrix.At(1, 1, eIotaLambda);
    }

    public override bool IsMutable => true;

    public override double Angle { get; set; }

    public override Matrix<Complex> Matrix => this.matrix;

    public override string Description => "The Phase Gate. (phase angle editable)";

    public override string Name => "Phase";

    public override string AlternateName => "Phase Gate";

    public override string CaptionKey { get; set; } = "Ph";

    public bool IsPiDivisor { get; private set; } = true;

    public int PiDivisor { get; private set; } = 2;

    public bool IsPositive { get; private set; } = true;

    public override string AngleParameterCaption { get; set; } = string.Empty;

    public override bool HasAngleParameter => true;

    public override GateCategory Category => GateCategory.PhaseParametrized;
}
