namespace Lyt.Quantics.Engine.Core;

// Cannot use Vector<Complex> using System.Numerics 
// but we still need System.Numerics for Complex 
// Be careful when using global usings 
using MathNet.Numerics.LinearAlgebra;

public sealed class QuBit : IEquatable<QuBit>
{
    private Vector<Complex> state;

    public QuBit(BasisState basisState)
    {
        var tensor = basisState.ToTensor();
        this.state = tensor.ToVector();
    }

    public Vector<Complex> State
    {
        get => this.state;
        set => this.state = value;
    }

    public bool IsCollapsed { get; private set; }

    public int CollapsedValue { get; private set; }

    public int Measure()
    {
        Complex complex = this.state[0];
        var conjugate = Complex.Conjugate(complex);
        double probability = (conjugate * complex).Real;
        double sample = RandomUtility.NextDouble();
        int result = 0;
        if (sample > probability)
        {
            result = 1;
        }

        Debug.WriteLine(result);

        this.IsCollapsed = true;
        this.CollapsedValue = result;
        return result;
    }

    public void Apply(Gate gate) => this.state = gate.Matrix.Multiply(this.state);

    public bool Equals(QuBit? other)
    {
        if ((other is null) || (this.state.Count != other.State.Count))
        {
            return false;
        }

        for (int i = 0; i < this.state.Count; ++i)
        {
            if (this.state[i] != other.State[i])
            {
                return false;
            }
        }

        return true;
    }

    public override bool Equals(object? obj) => this.Equals(obj as QuBit);

    public override int GetHashCode() => this.state.GetHashCode();
}
