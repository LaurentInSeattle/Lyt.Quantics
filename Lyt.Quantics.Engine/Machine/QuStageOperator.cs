namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStageOperator
{
    public QuStageOperator() { /* Required for serialization */ }

    public QuStageOperator(string gateKey)
    {
        this.GateKey = gateKey;
        this.GateParameters = new();
    }

    public QuStageOperator(Gate gate)
    {
        this.GateKey = gate.CaptionKey;
        this.GateParameters = new(gate);
    }

    /// <summary> Gate Identifier </summary>
    public string GateKey { get; set; } = "I";

    public GateParameters GateParameters { get; set; } = new();

    public List<int> QuBitIndices { get; set; } = [];

    [JsonIgnore]
    public Matrix<Complex> StageOperatorMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1, 1);

    public QuStageOperator DeepClone()
    {
        var clone = new QuStageOperator
        {
            GateKey = this.GateKey,
            GateParameters = ReflectionUtilities.CreateAndCopyPropertiesFrom(this.GateParameters)
        };
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
            this.StageOperatorMatrix = 
                GateFactory.Produce(this.GateKey, this.GateParameters).Matrix;
        }
        catch (Exception ex)
        {
            message = string.Concat("Exception thrown: " + ex.Message);
            return false;
        }

        return true;
    }
}