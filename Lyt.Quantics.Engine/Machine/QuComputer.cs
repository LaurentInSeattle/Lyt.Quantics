namespace Lyt.Quantics.Engine.Machine; 

public sealed class QuComputer
{
    public QuComputer() { /* Required for deserialization */ }

    public string Name { get; set; } = "< Untitled >";

    public string Description { get; set; } = "< Undocumented >";

    public int QuBitsCount { get; set; }

    public List<QuState> InitialStates { get; set; } = [];

    public List<QuStage> Stages { get; set; }= [];

    [JsonIgnore]
    public List<QuRegister> Registers { get; set; } = [];

    public bool Validate()
    {
        return true; 
    }

    public bool Prepare () 
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

    public static QuComputer Example =
        new QuComputer()
        {
            Name = "Entanglement", 
            Description = "Hadamard followed by CNot",
            QuBitsCount = 2,
            InitialStates = new List<QuState>() {  QuState.Zero , QuState.One },
            Stages = new List<QuStage>()
            {
                new QuStage()
                {                   
                   Operators =  
                        new List <QuStageOperator> () 
                        {
                             new QuStageOperator { GateKey = "H" , Inputs = [0] , Outputs = [0] },
                             new QuStageOperator { GateKey = "I" , Inputs = [1] , Outputs = [1] },
                        } ,
                },
                new QuStage()
                {
                   Operators =
                        new List <QuStageOperator> ()
                        {
                             new QuStageOperator { GateKey = "CX" , Inputs = [0,1] , Outputs = [0,1] },
                        } ,
                },
            },
        };

    #endregion   Do NOT Delete ~~~ Used for Unit Tests Serialization 
}
