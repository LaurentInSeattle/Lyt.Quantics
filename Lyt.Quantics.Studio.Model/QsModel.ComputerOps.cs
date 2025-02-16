namespace Lyt.Quantics.Studio.Model;

using static ModelStructureUpdateMessage;

public sealed partial class QsModel : ModelBase
{
    public Tuple<string, double, int>[] ReducedBitValuesProbabilities(QuRegister register)
    {
        var measureStates = this.QuBitMeasureStates;
        DumpMeasureStates(measureStates);
        var bitValuesProbabilities = register.BitValuesProbabilities();
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
            filteredBitValuesProbabilities[index] = new Tuple<string, double,int>(key, value, 0);
            ++index; 
        }

        return filteredBitValuesProbabilities;
    }

    public bool AddQubitAtEnd(out string message)
    {
        bool status = this.QuComputer.AddQubitAtEnd(out message);
        if (status)
        {
            this.QuBitMeasureStates.Add(true);
            this.IsDirty = true;
            this.Messenger.Publish(MakeQubitsChanged());
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool RemoveLastQubit(out string message)
    {
        bool status = this.QuComputer.RemoveLastQubit(out message);
        if (status)
        {
            this.QuBitMeasureStates.RemoveAt(this.QuBitMeasureStates.Count - 1);
            this.IsDirty = true;
            this.Messenger.Publish(MakeQubitsChanged());
        }
        else
        {
            this.PublishError(message);
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
        this.Messenger.Publish(new ModelMeasureStatesUpdateMessage());
        return true;
    }

    public bool PackStages(out string message)
    {
        bool status = this.QuComputer.PackStages(out message);
        if (status)
        {
            this.IsDirty = true;
            this.Messenger.Publish(MakeStagePacked());
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool UpdateQubit(int index, QuState newState, out string message)
    {
        bool status = this.QuComputer.UpdateQubit(index, newState, out message);
        if (status)
        {
            this.Messenger.Publish(new ModelResultsUpdateMessage());
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool AddGate(int stageIndex, QubitsIndices qubitsIndices, Gate gate, bool isDrop, out string message)
    {
        bool status = this.QuComputer.AddGate(stageIndex, qubitsIndices, gate, isDrop, out message);
        if (status)
        {
            this.IsDirty = true;
            this.Messenger.Publish(MakeStageChanged(stageIndex));
        }
        else
        {
            this.PublishError(message);
        }

        return status;
    }

    public bool RemoveGate(int stageIndex, QubitsIndices qubitsIndices, out string message)
    {
        bool status = this.QuComputer.RemoveGate(stageIndex, qubitsIndices, out message);
        if (status)
        {
            this.IsDirty = true;
            this.Messenger.Publish(MakeStageChanged(stageIndex));
        }
        else
        {
            this.PublishError(message);
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
                        this.Messenger.Publish(new ModelResetMessage());
                        this.Messenger.Publish(new ModelResultsUpdateMessage());
                        return true;
                    }
                }
            }
        }

        this.PublishError(message);
        return false;
    }

    public bool Run(bool runUsingKroneckerProduct, bool runAsync = false )
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
                    void PublishUpdate(bool isComplete, int step)
                        => this.Messenger.Publish(new ModelProgressMessage(IsComplete: isComplete, Step: step));

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
                            this.Messenger.Publish(new ModelResultsUpdateMessage());
                            return true;
                        }
                    }
                }
            }
        }

        this.PublishError(message);
        return false;
    }

    public async Task<Tuple<bool, string>> Break () => await this.QuComputer.Break();

    private void PublishError(string message)
        => this.Messenger.Publish(new ModelUpdateErrorMessage(message));

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
