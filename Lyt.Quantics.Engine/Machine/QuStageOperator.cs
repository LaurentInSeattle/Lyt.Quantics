namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStageOperator
{
    public QuStageOperator() { /* Required for serialization */ }

    /// <summary> 
    /// This CTOR only used to simulate serialization and to fill up a stage with Identity gates 
    /// </summary>
    /// <param name="gateKey"></param>
    public QuStageOperator(string gateKey)
    {
        this.GateKey = gateKey;
        this.GateParameters = new();
    }


    public QuStageOperator(Gate gate, int qubitIndex)
    {
        this.GateKey = gate.CaptionKey;
        this.GateParameters = new(gate);
        if (gate.QuBitsTransformed == 1)
        {
            // Unnary gate: the only one qubit is target 
            this.TargetQuBitIndices.Add(qubitIndex);
        }
        else if (gate.QuBitsTransformed == 2)
        {
            // Binary or ternary gate 
            if (gate.IsControlled)
            {

            }
            else
            {
                // For NOW 
                this.TargetQuBitIndices.Add(qubitIndex);
                this.TargetQuBitIndices.Add(1 + qubitIndex);
            }
        }
        else if (gate.QuBitsTransformed == 3)
        {
            // Ternary gate 
            // For NOW 
            this.TargetQuBitIndices.Add(qubitIndex);
            this.TargetQuBitIndices.Add(1 + qubitIndex);
        }
        else
        {
            throw new NotSupportedException("No gates processing more than 3 qubits.");
        } 
    }

    /// <summary> Gate Identifier </summary>
    public string GateKey { get; set; } = "I";

    public GateParameters GateParameters { get; set; } = new();

    /// <summary> Control Qubit Indices for controlled gates </summary>
    public List<int> ControlQuBitIndices { get; set; } = [];

    /// <summary> Target Qubit Indices for controlled gates and non controlled ones</summary>
    public List<int> TargetQuBitIndices { get; set; } = [];

    [JsonIgnore]
    public List<int> AllQuBitIndicesSorted
    {
        get
        {
            var allIndices = new List<int>();
            allIndices.AddRange(this.ControlQuBitIndices);
            allIndices.AddRange(this.TargetQuBitIndices);
            return [.. (from index in allIndices orderby index ascending select index)];
        }
    }

    [JsonIgnore]
    public int SmallestQubitIndex => this.AllQuBitIndicesSorted[0];

    [JsonIgnore]
    public Matrix<Complex> StageOperatorMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1, 1);

    public QuStageOperator DeepClone()
    {
        var clone = new QuStageOperator
        {
            GateKey = this.GateKey,
            GateParameters = ReflectionUtilities.CreateAndCopyPropertiesFrom(this.GateParameters)
        };

        foreach (int index in this.TargetQuBitIndices)
        {
            clone.TargetQuBitIndices.Add(index);
        }

        foreach (int index in this.ControlQuBitIndices)
        {
            clone.ControlQuBitIndices.Add(index);
        }

        return clone;
    }

    public bool Validate(QuComputer computer, out string message)
    {
        if (!GateFactory.AvailableProducts.ContainsKey(this.GateKey))
        {
            message = "Stage operator is using an unknown gate key: " + this.GateKey;
            return false;
        }

        int totalQubits = this.TargetQuBitIndices.Count + this.ControlQuBitIndices.Count;
        if (totalQubits == 0)
        {
            message = "No qubits indices provided for gate: " + this.GateKey;
            return false;
        }

        if (totalQubits > 3)
        {
            message =
                "Too many indices provided: " + totalQubits.ToString() +
                " for gate: " + this.GateKey;
            return false;
        }

        var gate = GateFactory.Produce(this.GateKey, this.GateParameters); 
        if (this.TargetQuBitIndices.Count != gate.TargetQuBits)
        {
            message = "Target qubits indices count not matching gate: " + this.GateKey;
            return false;
        }

        if (this.ControlQuBitIndices.Count != gate.ControlQuBits)
        {
            message = "Control qubits indices count not matching gate: " + this.GateKey;
            return false;
        }

        // Verify that the provided indices are all within range 
        bool Verify(List<int> indices, out string message )
        {
            message = string.Empty;
            foreach (int index in indices)
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

        if (!Verify(this.TargetQuBitIndices, out message))
        {
            return false;
        }

        if (!Verify(this.ControlQuBitIndices, out message))
        {
            return false;
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