﻿namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStageOperator
{
    public QuStageOperator()  { /* Required for serialization   */ }

    /// <summary> 
    /// This CTOR only used to simulate serialization and to fill up a stage with Identity gates 
    /// </summary>
    /// <param name="gateKey"></param>
    public QuStageOperator(string gateKey) : this()
    {
        this.GateKey = gateKey;
        this.GateParameters = new();
    }

    /// <summary> Regular CTOR </summary>
    /// <param name="gate"></param>
    /// <param name="qubitsIndices"></param>
    /// <param name="isDrop"></param>
    public QuStageOperator(Gate gate, QubitsIndices qubitsIndices) : this()
    {
        this.GateKey = gate.CaptionKey;
        this.GateParameters = new(gate);

        // Make a copy of provided qubits indices 
        this.TargetQuBitIndices = [.. qubitsIndices.TargetQuBitIndices];
        this.ControlQuBitIndices = [.. qubitsIndices.ControlQuBitIndices];
    }

    #region Public (must have get + set) Serialized Properties 

    /// <summary> Gate Identifier, defaults to Identity  </summary>
    public string GateKey { get; set; } = IdentityGate.Key;

    /// <summary> Gate Angle and Axis parameters </summary>
    public GateParameters GateParameters { get; set; } = new();

    /// <summary> Control Qubit Indices for most controlled gates, some have none. </summary>
    public List<int> ControlQuBitIndices { get; set; } = [];

    /// <summary> Target Qubit Indices for controlled gates and non controlled ones</summary>
    public List<int> TargetQuBitIndices { get; set; } = [];

    #endregion Public (get+set) Serialized Properties 

    [JsonIgnore]
    public bool IsIdentity => this.GateKey == IdentityGate.Key;

    [JsonIgnore]
    public int QuBitsTransformed => MathUtilities.IntegerLog2(this.StageOperatorMatrix.RowCount);

    [JsonIgnore]
    public bool HasBinarySwap =>
        (this.QuBitsTransformed == 2) && (this.LargestQubitIndex - this.SmallestQubitIndex > 1);

    [JsonIgnore]
    public bool HasTernarySwap =>
        (this.QuBitsTransformed == 3) &&
            ((this.LargestQubitIndex - this.SmallestQubitIndex > 2) ||
             (this.MiddleQubitIndex - this.SmallestQubitIndex > 1));

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
    public int MiddleQubitIndex =>
        this.QuBitsTransformed != 3 ?
            throw new InvalidOperationException("Not a ternary gate") :
            this.AllQuBitIndicesSorted[1];

    [JsonIgnore]
    public int LargestQubitIndex => this.AllQuBitIndicesSorted[^1];

    [JsonIgnore]
    public Gate StageOperatorGate { get; private set; } = GateFactory.Produce(IdentityGate.Key); 

    [JsonIgnore]
    public Matrix<Complex> StageOperatorMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1, 1);

    public QuStageOperator DeepClone()
    {
        var clone = new QuStageOperator
        {
            GateKey = this.GateKey,
            GateParameters = this.GateParameters.DeepClone(),
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
        bool Verify(List<int> indices, out string message)
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

    public bool Build(out string message)
    {
        message = string.Empty;
        try
        {
            this.StageOperatorGate = GateFactory.Produce(this.GateKey, this.GateParameters);
            this.StageOperatorMatrix = this.StageOperatorGate.Matrix;
        }
        catch (Exception ex)
        {
            message = string.Concat("Exception thrown: " + ex.Message);
            return false;
        }

        return true;
    }
}
