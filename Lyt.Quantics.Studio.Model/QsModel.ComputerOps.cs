﻿namespace Lyt.Quantics.Studio.Model;

using static ModelStructureUpdateMessage;

public sealed partial class QsModel : ModelBase
{
    [JsonIgnore]
    public bool HideMinibarsComputerState { get; set; }

    [JsonIgnore]
    public bool HideMinibarsUserOption { get; set; }

    [JsonIgnore]
    public QuComputer QuComputer { get; private set; }

    [JsonIgnore]
    public List<bool> QuBitMeasureStates { get; private set; } = [];

    public bool ShouldMeasureAllQubits
    {
        get
        {
            int total = this.QuBitMeasureStates.Count;
            if (total != this.QuComputer.QuBitsCount)
            {
                return false;
            }

            int countMeasured =
                (from state in this.QuBitMeasureStates where state select state).Count();
            return countMeasured == total;
        }
    }

    public bool ShouldMeasureNoQubits
    {
        get
        {
            int total = this.QuBitMeasureStates.Count;
            if (total != this.QuComputer.QuBitsCount)
            {
                return false;
            }

            int countMeasured =
                (from state in this.QuBitMeasureStates where state select state).Count();
            return countMeasured == 0;
        }
    }

    public HashSet<int> NonMeasuredQubitsIndices
    {
        get
        {
            HashSet<int> indices = [];
            for (int i = 0; i < this.QuBitMeasureStates.Count; ++i)
            {
                if (!this.QuBitMeasureStates[i])
                {
                    indices.Add(i);
                }
            }

            return indices;
        }
    }

    public List<Tuple<string, double>> ReducedBitValuesProbabilities(QuRegister register)
    {
        var measureStates = this.QuBitMeasureStates;
        DumpMeasureStates(measureStates);
        List<Tuple<string, double>> bitValuesProbabilities = register.BitValuesProbabilities();
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
        List<Tuple<string, double>> filteredBitValuesProbabilities = [];
        foreach (string key in keys)
        {
            double value = reducedBitValuesProbabilities[key];
            filteredBitValuesProbabilities.Add(new Tuple<string, double>(key, value));
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

    public bool Run(bool runSingleStage)
    {
        this.QuComputer.RunSingleStage = runSingleStage;
        bool status = this.QuComputer.Validate(out string message);
        if (status)
        {
            status = this.QuComputer.Build(out message);
            if (status)
            {
                status = this.QuComputer.Prepare(out message);
                if (status)
                {
                    status = this.QuComputer.Run(checkExpected: false, out message);
                    if (status)
                    {
                        this.Messenger.Publish(new ModelResultsUpdateMessage());
                        return true;
                    }
                }
            }
        }

        this.PublishError(message);
        return false;
    }

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
