using Lyt.Quantics.Engine.Gates.UnaryParametrized;

namespace Lyt.Quantics.Engine.Gates.Base;

public sealed class GateParameters
{
    public GateParameters() { /* for serialisation and default */ }

    public GateParameters(Gate gate)
    {
        if (gate is RotationGate rotationGate)
        {
            this.Axis = rotationGate.Axis;

            this.Angle = rotationGate.Angle;
            this.IsPiDivisor = rotationGate.IsPiDivisor;
            this.PiDivisor = rotationGate.PiDivisor;
            this.IsPositive = rotationGate.IsPositive;
        }

        if (gate is PhaseGate phaseGate)
        {
            this.Angle = phaseGate.Angle;
            this.IsPiDivisor = phaseGate.IsPiDivisor;
            this.PiDivisor = phaseGate.PiDivisor;
            this.IsPositive = phaseGate.IsPositive;
        }
    }

    /// <summary> Parameter for Rotation Gates </summary>
    public Axis Axis { get; set; } = Axis.X;

    /// <summary> Parameter for Phase Gate and Rotation Gates </summary>
    public double Angle { get; set; } = 0.0;

    public bool IsPiDivisor { get; set; } = true;

    public int PiDivisor { get; set; } = 2;

    public bool IsPositive { get; set; } = true;
}