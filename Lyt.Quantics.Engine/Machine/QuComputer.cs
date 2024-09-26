namespace Lyt.Quantics.Engine.Machine;

public sealed class QuComputer
{
    public QuComputer() { /* Required for deserialization */ }

    public string Name { get; set; } = "< Untitled >";

    public string Description { get; set; } = "< Undocumented >";

    public int QuBitsCount { get; set; }

    public List<QuState> InitialStates { get; set; } = [];

    public List<QuStage> Stages { get; set; } = [];

    [JsonIgnore]
    public List<QuRegister> Registers { get; set; } = [];

    public bool Validate(out string message)
    {
        message = string.Empty;
        if( string.IsNullOrWhiteSpace(this.Name) )
        {
            message = "Validate: QuComputer has no name"; 
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.Description))
        {
            message = "Validate: QuComputer has no documentation";
            return false;
        }

        if (this.QuBitsCount <= 0)
        {
            message = "Validate: Invalid QuBit count.";
            return false;
        }

        if (this.QuBitsCount > 10)
        {
            message = "Validate: Intractable QuBit count. (>10)";
            return false;
        }

        if (this.InitialStates.Count != this.QuBitsCount)
        {
            message = "Validate: Count of initial states does not match QuBit count.";
            return false;
        }

        if (this.Stages.Count == 0)
        {
            message = "Validate: No stages defined.";
            return false;
        }

        int stageIndex = 0 ;
        foreach (QuStage stage in this.Stages)
        {
            if ( ! stage.Validate(out message))
            {
                string prefix = string.Format("Validate: At stage index {0}: ", stageIndex);
                message = string.Concat(prefix, message);
                return false;
            }

            ++stageIndex;
        }

        return true;
    }

    public bool Build(out string message)
    {
        message = string.Empty;

        int stageIndex = 0;
        foreach (QuStage stage in this.Stages)
        {
            if (!stage.Build(this, out message))
            {
                string prefix = string.Format("Build: At stage index {0}: ", stageIndex);
                message = string.Concat(prefix, message);
                return false;
            }

            ++stageIndex;
        }
        return true;
    }

    public bool Prepare()
    {
        return true;
    }

    public bool Step()
    {
        return true;
    }

    public bool Run()
    {
        return true;
    }


    #region LATER 

    //public bool AddStage(QuStage stage)
    //{
    //    // TODO 
    //    this.Stages.Add(stage);
    //    return true;
    //}

    //public bool RemoveStage(QuStage stage)
    //{
    //    // TODO 
    //    this.Stages.Remove(stage);
    //    return true;
    //}

    #endregion LATER 

    #region   Do NOT Delete ~~~ Used for Unit Tests Serialization 

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static QuComputer Example =
#pragma warning restore CA2211 
        new()
        {
            Name = "Entanglement",
            Description = "Hadamard followed by CNot",
            QuBitsCount = 2,
            InitialStates = [QuState.Zero, QuState.One],
            Stages =
            [
                new QuStage()
                {
                   Operators =
                    [
                            new QuStageOperator { GateKey = "H" , QuBitIndices = [0]  },
                            new QuStageOperator { GateKey = "I" , QuBitIndices = [1]  },
                    ] ,
                },
                new QuStage()
                {
                   Operators =
                    [
                            new QuStageOperator { GateKey = "CX" , QuBitIndices = [0,1]},
                    ] ,
                },
            ],
        };

    #endregion   Do NOT Delete ~~~ Used for Unit Tests Serialization 
}
