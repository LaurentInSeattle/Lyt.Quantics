using Lyt.Quantics.Engine.Gates.Base;

namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStageOperator
{
    public List<int> QuBitIndices { get; set; } = [];

    public string GateKey { get; set; } = IdentityGate.Key;

    [JsonIgnore]
    public Matrix<Complex> StageOperatorMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1, 1);

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
            this.StageOperatorMatrix = GateFactory.Produce( this.GateKey).Matrix;
        }
        catch (Exception ex)
        {
            message = string.Concat("Exception thrown: " + ex.Message);
            return false;
        }

        return true;
    }
} 