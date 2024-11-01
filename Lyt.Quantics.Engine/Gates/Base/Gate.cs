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

    /// <summary> The matrix dimension. </summary>
    public int Dimension => this.Matrix.RowCount;

    /// <summary> How qubits this gate will transform. </summary>
    public int QuBits => MathUtilities.IntegerLog2(this.Matrix.RowCount);

    /// <summary> Convenience human readable info.</summary>
    public abstract string Name { get; }

    /// <summary> Convenience human readable info.</summary>
    public abstract string AlternateName { get; }

    /// <summary> Documentation about the gate </summary>
    public virtual string Documentation { get; set; } = string.Empty;
 
    /// <summary> Gate category </summary>
    public abstract GateCategory Category { get; }

    /// <summary> True if the gate is constructed with an angle parameter </summary>
    public virtual bool IsParametrized => false;

    /// <summary> Parameter string for the gate.</summary>
    public virtual string ParameterCaption { get; set; } = string.Empty;

    /// <summary> True if the gate is controlling, if control and target qubit indices can be edited </summary>
    /// <remarks> Always false for now. </remarks>
    public virtual bool IsControlling => false;
}
