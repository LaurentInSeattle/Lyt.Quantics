namespace Lyt.Quantics.Studio.Model;

using static ModelStructureUpdateMessage;

public sealed partial class QsModel : ModelBase
{
    public Tuple<string, double, int>[] ReducedBitValuesProbabilities(QuRegister register)
    {
        var measureStates = this.QuBitMeasureStates;
        DumpMeasureStates(measureStates);
        var bitValuesProbabilities = register.BitValuesProbabilities;
        if (this.ShouldMeasureAllQubits)
        {
            // All qubits measured, return the full list 
            return bitValuesProbabilities;
        }

        HashSet<int> nonMeasured = this.NonMeasuredQubitsIndices;

        string ReduceBitString(string source)
        {
            StringBuilder sb = new();
            for (int i = 0; i < source.Length; ++i)
            {
                int j = source.Length - 1 - i;
                if (!nonMeasured.Contains(i))
                {
                    sb.Append(source[j]);
                }
            }

            string result = sb.ToString();
            result = result.Reverse();
            return result;
        }

        Dictionary<string, double> reducedBitValuesProbabilities = [];
        foreach (var bitValue in bitValuesProbabilities)
        {
            string reduced = ReduceBitString(bitValue.Item1);
            if (reducedBitValuesProbabilities.TryGetValue(reduced, out double value))
            {
                reducedBitValuesProbabilities[reduced] = value + bitValue.Item2;
            }
            else
            {
                reducedBitValuesProbabilities.Add(reduced, bitValue.Item2);
            }
        }

        List<string> keys = [];
        foreach (var kvp in reducedBitValuesProbabilities)
        {
            keys.Add(kvp.Key);
        }

        keys.Sort();
        keys.Reverse();
        var filteredBitValuesProbabilities = new Tuple<string, double, int>[keys.Count];
        int index = 0;
        foreach (string key in keys)
        {
            double value = reducedBitValuesProbabilities[key];
            filteredBitValuesProbabilities[index] = new Tuple<string, double, int>(key, value, 0);
            ++index;
        }

        return filteredBitValuesProbabilities;
    }

    public bool AddQubitAtEnd(out string message)
    {
        bool status = this.QuComputer.AddQubitAtEnd(out message);
        if (status)
        {
            SwapData.OnQuBitCountChanged(this.QuComputer.QuBitsCount);
            this.QuBitMeasureStates.Add(true);
            this.IsDirty = true;
            MakeQubitsChanged().Publish();
        }
        else
        {
            PublishError(message);
        }

        return status;
    }

    public bool RemoveLastQubit(out string message)
    {
        bool status = this.QuComputer.RemoveLastQubit(out message);
        if (status)
        {
            SwapData.OnQuBitCountChanged(this.QuComputer.QuBitsCount);
            this.QuBitMeasureStates.RemoveAt(this.QuBitMeasureStates.Count - 1);
            this.IsDirty = true;
            MakeQubitsChanged().Publish();
        }
        else
        {
            PublishError(message);
        }

        return status;
    }

    public bool UpdateQubitMeasureState(int index, bool value, out string message)
    {
        message = string.Empty;
        if ((index < 0) || (index >= this.QuBitMeasureStates.Count))
        {
            message = "Invalid qubit index";
            return false;
        }

        this.QuBitMeasureStates[index] = value;
        new ModelMeasureStatesUpdateMessage().Publish();
        return true;
    }

    public bool PackStages(out string message)
    {
        bool status = this.QuComputer.PackStages(out message);
        if (status)
        {
            this.IsDirty = true;
            MakeStagePacked().Publish();
        }
        else
        {
            PublishError(message);
        }

        return status;
    }

    public bool UpdateQubit(int index, QuState newState, out string message)
    {
        bool status = this.QuComputer.UpdateQubit(index, newState, out message);
        if (status)
        {
            new ModelResultsUpdateMessage().Publish();
        }
        else
        {
            PublishError(message);
        }

        return status;
    }

    public bool AddGate(int stageIndex, QubitsIndices qubitsIndices, Gate gate, bool isDrop, out string message)
    {
        if (gate is AntiControlledNotGate)
        {
            return this.AddAntiControlledNotGate(stageIndex, qubitsIndices, isDrop, out message);
        }

        bool status = this.QuComputer.AddGate(stageIndex, qubitsIndices, gate, isDrop, out message);
        if (status)
        {
            this.IsDirty = true;
            MakeStageChanged(stageIndex).Publish();
        }
        else
        {
            PublishError(message);
        }

        return status;
    }

    public bool AddAntiControlledNotGate(int stageIndex, QubitsIndices qubitsIndices, bool isDrop, out string message)
    {
        var xGate1 = GateFactory.Produce(PauliXGate.Key);
        bool status1 = this.QuComputer.AddGate(stageIndex, qubitsIndices, xGate1, isDrop, out message);
        if ( ! status1)
        {
            PublishError(message);
            return false ;
        }

        var gate = GateFactory.Produce(ControlledNotGate.Key);
        bool status2 = this.QuComputer.AddGate(1 + stageIndex, qubitsIndices, gate, isDrop, out message);
        if (!status2)
        {
            PublishError(message);
            return false;
        }

        var xGate2 = GateFactory.Produce(PauliXGate.Key);
        bool status3 = this.QuComputer.AddGate(2 + stageIndex, qubitsIndices, xGate2, isDrop, out message);
        if (!status3)
        {
            PublishError(message);
            return false;
        }

        bool status = status1 && status2 && status3;
        if (status)
        {
            this.IsDirty = true;
            MakeModelLoaded().Publish();
        }
        else
        {
            PublishError(message);
        }

        return status;
    }

    public bool RemoveGate(int stageIndex, QubitsIndices qubitsIndices, out string message)
    {
        bool status = this.QuComputer.RemoveGate(stageIndex, qubitsIndices, out message);
        if (status)
        {
            this.IsDirty = true;
            MakeStageChanged(stageIndex).Publish();
        }
        else
        {
            PublishError(message);
        }

        return status;
    }

    public bool Reset()
    {
        bool status = this.QuComputer.Reset(out string message);
        if (status)
        {
            this.QuComputer.Validate(out message);
            if (status)
            {
                status = this.QuComputer.Build(out message);
                if (status)
                {
                    status = this.QuComputer.Prepare(out message);
                    if (status)
                    {
                        new ModelResetMessage().Publish();
                        new ModelResultsUpdateMessage().Publish();
                        return true;
                    }
                }
            }
        }

        PublishError(message);
        return false;
    }

    public bool Run(bool runUsingKroneckerProduct, bool runAsync = false)
    {
        this.QuComputer.RunUsingKroneckerProduct = runUsingKroneckerProduct;
        bool status = this.QuComputer.Validate(out string message);
        if (status)
        {
            status = this.QuComputer.Build(out message);
            if (status)
            {
                status = this.QuComputer.Prepare(out message);
                if (status)
                {
                    static void PublishUpdate(bool isComplete, int step)
                        => new ModelProgressMessage(IsComplete: isComplete, Step: step).Publish();

                    if (runAsync)
                    {
                        // Fire and forget the calculation task 
                        _ = Task.Run(() => this.QuComputer.RunAsync(checkExpected: false, PublishUpdate));
                        return true;
                    }
                    else
                    {
                        status = this.QuComputer.Run(checkExpected: false, PublishUpdate, out message);
                        if (status)
                        {
                            new ModelResultsUpdateMessage().Publish();
                            return true;
                        }
                    }
                }
            }
        }

        PublishError(message);
        return false;
    }

    public async Task<Tuple<bool, string>> Break() => await this.QuComputer.Break();

    private static void PublishError(string message)
        => new ModelUpdateErrorMessage(message).Publish();

    private void InitializeMeasureStates()
    {
        this.QuBitMeasureStates.Clear();
        for (int i = 0; i < this.QuComputer.QuBitsCount; i++)
        {
            this.QuBitMeasureStates.Add(true);
        }
    }

    [Conditional("DEBUG")]
    private static void DumpMeasureStates(List<bool> measureStates)
    {
        Debug.Write("Measure States: ");
        foreach (bool measureState in measureStates)
        {
            Debug.Write(measureState ? " * " : " . ");
        }
        Debug.WriteLine(" ");
    }
}
