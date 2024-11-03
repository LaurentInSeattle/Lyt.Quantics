namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStageOperator
{
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    public QuStageOperator() { /* Required for serialization */ }
#pragma warning restore CS8618 

    public QuStageOperator(string gateKey)
        => this.Gate = GateFactory.Produce(gateKey);

    public QuStageOperator(Gate gate)
        => this.Gate = QuStageOperator.ProduceGate(gate);

    public List<int> QuBitIndices { get; set; } = [];

    public Gate Gate { get; private set; }

    [JsonIgnore]
    public string GateKey => this.Gate is null ? "I" : this.Gate.CaptionKey;

    [JsonIgnore]
    public Matrix<Complex> StageOperatorMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1, 1);

    public QuStageOperator DeepClone()
    {
        // Need to clone the gate
        var clone = new QuStageOperator(QuStageOperator.ProduceGate(this.Gate));
        foreach (int index in QuBitIndices)
        {
            clone.QuBitIndices.Add(index);
        }

        return clone;
    }

    public bool Validate(QuComputer computer, out string message)
    {
        message = string.Empty;

        if (!GateFactory.AvailableProducts.ContainsKey(this.GateKey))
        {
            message = "Stage operator is using an unknown gate key: " + this.GateKey;
            return false;
        }

        if (this.QuBitIndices.Count == 0)
        {
            message = "No indices provided for gate: " + this.GateKey;
            return false;
        }

        if (this.QuBitIndices.Count > 3)
        {
            message =
                "Too many indices provided: " + this.QuBitIndices.Count.ToString() +
                " for gate: " + this.GateKey;
            return false;
        }

        // Verify that the provided indices are within range 
        foreach (int index in this.QuBitIndices)
        {
            if ((index < 0) || (index >= computer.QuBitsCount))
            {
                message =
                    "Some provided qubit indices are not within range, index: " + index.ToString() +
                    " for gate: " + this.GateKey;
                return false;
            }
        }

        return true;
    }

    public bool Build(QuComputer _, out string message)
    {
        message = string.Empty;

        try
        {
            this.StageOperatorMatrix = GateFactory.Produce(this.GateKey).Matrix;
        }
        catch (Exception ex)
        {
            message = string.Concat("Exception thrown: " + ex.Message);
            return false;
        }

        return true;
    }

    private static Gate ProduceGate(Gate gate)
    {
        if (gate is PhaseGate phaseGate)
        {
            return GateFactory.Produce(gate.CaptionKey, axis: Axis.X, phaseGate.Angle);
        }
        else if (gate is RotationGate rotationGate)
        {
            return GateFactory.Produce(gate.CaptionKey, rotationGate.Axis, rotationGate.Angle);
        }
        else
        {
            return GateFactory.Produce(gate.CaptionKey);
        }
    }
}