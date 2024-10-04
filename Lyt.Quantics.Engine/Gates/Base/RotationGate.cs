﻿namespace Lyt.Quantics.Engine.Gates.Base;

public class RotationGate : Gate
{
    // See: https://www.quantum-inspire.com/kbase/rotation-operators/ 
    //
    // The rotation operators are defined as:
    //
    // Rx(θ)= {  cos⁡(θ/2)    −isin⁡(θ/2)  } 
    //        {  −isin(θ/2​)  cos(θ/2​)​     } 

    // Ry(θ)=  {  cos⁡(θ/2)   −sin⁡(θ/2)   }    
    //         {  sin(θ/2​)   cos(θ/2​)    } 

    // Rz(θ)=  {  e−iθ/2     0     } 
    //         {  0          eiθ/2​ }​

    // The rotation operators are generated by exponentiation of the Pauli matrices according to
    //  exp(iAx)=cos⁡(x)I+isin⁡(x)A exp(iAx)=cos(x)I+isin(x)A  where A is one of the three
    //  Pauli Matrices.

    private readonly Matrix<Complex> matrix ;
    private readonly string captionKey ;

    public RotationGate(Axis axis, double theta = Math.PI / 2.0 )
    {
        this.Theta = theta; 
        this.RotationAxis = axis ;

        double half = theta / 2.0;
        double  sinReal = Math.Sin(half);
        double  cosReal = Math.Cos(half);
        Complex cosComplex = cosReal;
        Complex sinComplex = sinReal;
        Complex iotaSin = new Complex(0 , sinReal) ;

        this.matrix = Matrix<Complex>.Build.Sparse(2, 2, Complex.Zero);

        switch (axis)
        {
            default:
            case Axis.X:
                this.matrix.At(0, 0, cosComplex);
                this.matrix.At(0, 1, -iotaSin);
                this.matrix.At(1, 0, -iotaSin);
                this.matrix.At(1, 1, cosComplex);
                this.captionKey = "Rx"; 
                break;

            case Axis.Y:
                this.matrix.At(0, 0, cosComplex);
                this.matrix.At(0, 1, -sinComplex);
                this.matrix.At(1, 0, sinComplex);
                this.matrix.At(1, 1, cosComplex);
                this.captionKey = "Ry";
                break;

            case Axis.Z:
                var complex = new Complex(cosReal, sinReal);
                var conjugate = new Complex(cosReal, -sinReal);
                this.matrix.At(0, 0, conjugate);
                this.matrix.At(1, 1, conjugate);
                this.captionKey = "Rz";
                break;
        }
    }

    public Axis RotationAxis { get; private set; }

    public double Theta { get; private set; }

    public override Matrix<Complex> Matrix => this.matrix;

    public override string Name => "Rotation Gate";

    public override string AlternateName => "Rotation Operator";

    public override string CaptionKey => this.captionKey;
}
