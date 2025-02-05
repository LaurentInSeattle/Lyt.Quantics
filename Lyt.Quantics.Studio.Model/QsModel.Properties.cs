namespace Lyt.Quantics.Studio.Model;

public sealed partial class QsModel : ModelBase
{
    #region NOT serialized - WITH model changed event

    [JsonIgnore]
    public bool ShowBuiltInComputers { get => this.Get<bool>(); set => this.SetClean(value); }

    [JsonIgnore]
    public bool ShowRecentDocuments { get => this.Get<bool>(); set => this.SetClean(value); }

    #endregion NOT serialized - WITH model changed event

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

}
