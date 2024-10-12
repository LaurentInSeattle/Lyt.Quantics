namespace Lyt.Quantics.Engine.Gates.Base;

using MathNet.Numerics.LinearAlgebra;

/// <summary> Base class for all gates, regardless of dimension.</summary>
public abstract class Gate
{
    // See: https://en.wikipedia.org/wiki/List_of_quantum_logic_gates 

    /// <summary> Unique idenfifier for the gate.</summary>
    public abstract string CaptionKey { get; }

    /// <summary> Matrix of the gate, can be dense or sparse, Math.Net handles that for us. </summary>
    public abstract Matrix<Complex> Matrix { get; }

    public int Dimension => this.Matrix.RowCount;

    /// <summary> Convenience human readable info.</summary>
    public abstract string Name { get; }

    /// <summary> Convenience human readable info.</summary>
    public abstract string AlternateName { get; }

    /// <summary> TODO </summary>
    public virtual string Documentation { get; set; } = string.Empty;
 
    /// <summary> Gate category </summary>
    public abstract GateCategory Category { get; }
}
