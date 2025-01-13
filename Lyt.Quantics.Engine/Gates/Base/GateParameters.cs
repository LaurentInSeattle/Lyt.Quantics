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

        if ( gate is ControlledGate controlledGate)
        {
            Gate baseGate = controlledGate.BaseGate;
            this.BaseGateKey = baseGate.CaptionKey;

            if (baseGate is RotationGate baseRotationGate)
            {
                this.Axis = baseRotationGate.Axis;

                this.Angle = baseRotationGate.Angle;
                this.IsPiDivisor = baseRotationGate.IsPiDivisor;
                this.PiDivisor = baseRotationGate.PiDivisor;
                this.IsPositive = baseRotationGate.IsPositive;
            }

            if (baseGate is PhaseGate basePhaseGate)
            {
                this.Angle = basePhaseGate.Angle;
                this.IsPiDivisor = basePhaseGate.IsPiDivisor;
                this.PiDivisor = basePhaseGate.PiDivisor;
                this.IsPositive = basePhaseGate.IsPositive;
            }
        }
    }

    /// <summary> Parameter for Controlled Gates </summary>
    public string BaseGateKey { get; set; } = string.Empty;

    /// <summary> Parameter for Rotation Gates </summary>
    public Axis Axis { get; set; } = Axis.X;

    /// <summary> Parameters for Phase Gate and Rotation Gates </summary>
    public double Angle { get; set; } = 0.0;

    public bool IsPiDivisor { get; set; } = true;

    public int PiDivisor { get; set; } = 2;

    public bool IsPositive { get; set; } = true;
}