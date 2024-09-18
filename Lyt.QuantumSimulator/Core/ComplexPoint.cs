namespace Lyt.QuantumSimulator.Core;

public class ComplexPoint : IEquatable<ComplexPoint>
{
    public ComplexPoint(bool b)
    {
        this.X = b ? Complex.Zero : Complex.One;
        this.Y = b ? Complex.One : Complex.Zero;
    }

    public ComplexPoint(Complex x, Complex y)
    {
        this.X = x;
        this.Y = y;
    }

    public Complex X { get; private set; }

    public Complex Y { get; private set; }

    public double Magnitude() => Math.Sqrt(this.X.Magnitude * this.X.Magnitude + this.Y.Magnitude * this.Y.Magnitude );
    
    public void DivideBy(double divisor)
    {
        this.X /= divisor;
        this.Y /= divisor;
    }

    public override int GetHashCode() => new { this.X, this.Y }.GetHashCode();

    public bool Equals(ComplexPoint? other) => other is not null && other.X == this.X && other.Y == this.Y;

    public override bool Equals(object? obj) => this.Equals(obj as ComplexPoint);
}
