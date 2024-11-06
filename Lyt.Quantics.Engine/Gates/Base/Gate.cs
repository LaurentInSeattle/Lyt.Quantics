namespace Lyt.Quantics.Engine.Gates.Base;

using MathNet.Numerics.LinearAlgebra;

/// <summary> Base class for all gates, regardless of dimension.</summary>
public class Gate
{
    // See: https://en.wikipedia.org/wiki/List_of_quantum_logic_gates 

    /// <summary> Unique idenfifier for the gate.</summary>
    public virtual string CaptionKey { get; set; } = string.Empty;

    /// <summary> Parameter for Rotation Gates </summary>
    public virtual Axis Axis { get; set; } = Axis.X;

    /// <summary> Parameter for Phase Gate and Rotation Gates </summary>
    public virtual double Angle { get; set; } = 0.0;

    public virtual int ControlQuBits => 0;

    public virtual int TargetQuBits => 1;

    /// <summary> True if the gate is controlling, if control and target qubit indices can be edited </summary>
    /// <remarks> Always false for now. </remarks>
    public virtual bool IsControlling => false;

    /// <summary> The matrix dimension. </summary>
    public int MatrixDimension => this.Matrix.RowCount;

    /// <summary> How qubits this gate will transform. </summary>
    public int QuBitsTransformed => MathUtilities.IntegerLog2(this.Matrix.RowCount);

    /// <summary> Matrix of the gate, can be dense or sparse, Math.Net handles that for us. </summary>
    public virtual Matrix<Complex> Matrix { get; set; } = Matrix<Complex>.Build.Dense(1, 1);

    /// <summary> Convenience human readable info.</summary>
    public virtual string Name { get; set; } = string.Empty;

    /// <summary> Convenience human readable info.</summary>
    public virtual string AlternateName { get; set; } = string.Empty;

    /// <summary> Documentation about the gate </summary>
    public virtual string Documentation { get; set; } = string.Empty;

    /// <summary> Gate category </summary>
    public virtual GateCategory Category { get; set; }

    /// <summary> True if the gate is constructed with an angle parameter </summary>
    public virtual bool HasAngleParameter => false;

    /// <summary> Parameter string for the gate.</summary>
    public virtual string AngleParameterCaption { get; set; } = string.Empty;

    /// <summary> True if the gate is a Controlled gate</summary>
    public bool IsControlled => this.ControlQuBits >= 1 ;
}
