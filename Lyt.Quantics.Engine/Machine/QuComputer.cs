namespace Lyt.Quantics.Engine.Machine;

using MathNet.Numerics.LinearAlgebra;

public sealed partial class QuComputer
{
    private const string DefaultName = "< Untitled >";
    private const string DefaultDescription = "< Undocumented >";
    private const string DefaultComment = "< No comments >";

    private bool isValid;
    private bool isBuilt;
    private bool isPrepared;
    private bool isStepping;
    private bool isRunning;
    private bool isComplete;

    public QuComputer() { /* Required for deserialization */ }

    public QuComputer(string name, string description) 
    {
        this.Name = name;
        this.Description = description; 
    }

    #region JSON Properties 

    public string Name { get; set; } = DefaultName;

    public string Description { get; set; } = DefaultDescription;

    public string Comment { get; set; } = DefaultComment;

    public int QuBitsCount { get; set; }

    public List<QuState> InitialStates { get; set; } = [];

    public List<QuStage> Stages { get; set; } = [];

    /// <summary> 
    /// Expected Final Probabilities are provided for bit values (not kets) so that they 
    /// should match the results displayed in our 'official' QRyd simulator. 
    /// See Web page: https://designer.thequantumlaend.de/ 
    /// </summary>
    public List<double> ExpectedFinalProbabilities { get; set; } = [];

    #endregion JSON Properties 

    #region Runtime Properties not serialized

    [JsonIgnore]
    public QuRegister InitialRegister { get; private set; } = new(1);

    [JsonIgnore]
    public Vector<float> Result { get; private set; } = Vector<float>.Build.Dense(1);

    // TODO:
    // Use a finite state machine instead of all those potentially overlapping
    // and potentially inconsistent states.
    
    [JsonIgnore]
    public bool IsValid
    {
        get => this.isValid;
        private set
        {
            this.isValid = value;
            if ( ! value )
            {
                this.IsBuilt = false; 
            }
        } 
    }

    [JsonIgnore]
    public bool IsBuilt 
    { 
        get => this.isBuilt; 
        private set
        {
            this.isBuilt = value;
            if (!value)
            {
                this.IsPrepared = false;
            }
        }
    }

    [JsonIgnore]
    public bool IsPrepared 
    { 
        get => this.isPrepared; 
        private set
        {
            this.isPrepared = value;
            if (!value)
            {
                this.IsStepping = false;
                this.IsRunning = false;
                this.IsComplete = false;
            }
        }
    }

    [JsonIgnore]
    public bool IsStepping 
    { 
        get => this.isStepping; 
        private set => this.isStepping = value; 
    }

    [JsonIgnore]
    public bool IsRunning 
    { 
        get => this.isRunning; 
        private set => this.isRunning = value; 
    }

    [JsonIgnore]
    public bool IsComplete 
    { 
        get => this.isComplete; 
        private set => this.isComplete = value; 
    }

    // TODO: END 

    [JsonIgnore]
    public int StepIndex { get; private set; }

    #endregion Runtime Properties not serialized

    public bool Validate(out string message)
    {
        message = string.Empty;
        if (string.IsNullOrWhiteSpace(this.Name) || this.Name.Equals(DefaultName))
        {
            message = "Validate: QuComputer has no name";
            return false;
        }

        if (string.IsNullOrWhiteSpace(this.Description) || this.Description.Equals(DefaultDescription))
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

        if (this.ExpectedFinalProbabilities.Count != 0)
        {
            double sum = this.ExpectedFinalProbabilities.Sum();
            if (!sum.AlmostEqual(1.0))
            {
                message = "Validate: Sum of expected probabilities is not equal to 1.";
                return false;
            }
        }

        int stageIndex = 0;
        foreach (QuStage stage in this.Stages)
        {
            if (!stage.Validate(this, out message))
            {
                string prefix = string.Format("Validate: At stage index {0}: ", stageIndex);
                message = string.Concat(prefix, message);
                return false;
            }

            ++stageIndex;
        }

        this.IsValid = true;
        return true;
    }

    public bool Build(out string message)
    {
        message = string.Empty;
        try
        {
            // Build stages 
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
        }
        catch (Exception ex)
        {
            message = string.Concat("Build: Exception thrown: " + ex.Message);
            return false;
        }

        this.IsBuilt = true;
        return true;
    }

    public bool Prepare(List<QuState> initialStates, List<double> expectedFinalProbabilities, out string message)
    {
        if (initialStates.Count != this.QuBitsCount)
        {
            message = "Prepare: Count of initial states does not match QuBit count.";
            return false;
        }

        int length = this.ExpectedFinalProbabilities.Count;
        if (length != 0)
        {
            if (length != MathUtilities.TwoPower(this.QuBitsCount))
            {
                message = "Prepare: Invalid count of expected probabilities.";
                return false;
            }

            double sum = this.ExpectedFinalProbabilities.Sum();
            if (!sum.AlmostEqual(1.0))
            {
                message = "Prepare: Sum of expected probabilities is not equal to 1.";
                return false;
            }
        }

        this.InitialStates = initialStates;
        this.ExpectedFinalProbabilities = expectedFinalProbabilities;
        return this.Prepare(out message);
    }

    public bool Prepare(out string message)
    {
        message = string.Empty;
        try
        {
            this.IsComplete = false;
            this.IsRunning = false;

            // Setup initial register
            this.StepIndex = 0;
            this.InitialRegister = new QuRegister(this.InitialStates);
            Debug.WriteLine(string.Format("Initial State: {0}", this.InitialRegister.State));
            Debug.WriteLine(
                string.Format(
                    "Initial State Probabilities: {0}",
                    Vector<double>.Build.Dense(this.InitialRegister.KetProbabilities().ToArray())));
        }
        catch (Exception ex)
        {
            message = string.Concat("Prepare: Exception thrown: " + ex.Message);
            return false;
        }

        this.IsPrepared = true;
        return true;
    }

    public bool Step(out string message)
    {
        if (this.IsRunning)
        {
            message = "Step: Machine is running: Can't Step.";
            return false;
        }

        if (this.IsComplete)
        {
            message = "Step: Machine is in Complete State: Invoke Prepare before Stepping";
            return false;
        }

        if (!this.IsPrepared)
        {
            message = "Step: Machine has not been prepared: Invoke Prepare before Stepping";
            return false;
        }

        this.IsStepping = true;
        message = string.Empty;
        try
        {
            // stepping
            this.DoStep(out message);
        }
        catch (Exception ex)
        {
            message = string.Concat("Step: Exception thrown: " + ex.Message);
            return false;
        }
        finally
        {
            this.IsStepping = false;
        }

        return true;
    }

    private bool DoStep(out string message)
    {
        try
        {
            // Single Step
            var stage = this.Stages[this.StepIndex];
            Debug.WriteLine(string.Format("Step: {0}  {1}", this.StepIndex, stage.Operations));
            QuRegister sourceRegister =
                this.StepIndex == 0 ? this.InitialRegister : this.Stages[this.StepIndex - 1].StageRegister;
            stage.Calculate(sourceRegister, out message);
            if (!string.IsNullOrEmpty(message))
            {
                Debug.WriteLine(message);
                return false;
            }

            Debug.WriteLine(
                string.Format("Step: {0} - Probabilities: {1}", this.StepIndex, stage.KetProbabilities));
        }
        catch (Exception ex)
        {
            message = string.Concat("Step: Exception thrown: " + ex.Message);
            return false;
        }

        return true;
    }

    public bool Run(out string message)
    {
        if (this.IsRunning)
        {
            message = "Run: Machine is already running.";
            return false;
        }

        if (this.IsComplete)
        {
            message = "Run: Machine is in Complete State: Invoke Prepare before running";
            return false;
        }

        if (!this.IsPrepared)
        {
            message = "Run: Machine has not been prepared: Invoke Prepare before running";
            return false;
        }

        this.IsRunning = true;
        message = string.Empty;
        try
        {
            // running the stages 
            for (int i = 0; i < this.Stages.Count; i++)
            {
                if (this.DoStep(out message))
                {
                    ++this.StepIndex;
                }
                else
                {
                    return false;
                }
            }

            if (!this.AsExpected(out message))
            {
                return false;
            }

            // Measure last register
            QuRegister lastRegister = this.Stages[^1].StageRegister;
            Vector<float> measure = Vector<float>.Build.Dense([.. lastRegister.Measure()]);
            Debug.WriteLine("Last stage, measure: " + measure.ToString());
            this.Result = measure;
        }
        catch (Exception ex)
        {
            message = string.Concat("Step: Exception thrown: " + ex.Message);
            return false;
        }
        finally
        {
            this.IsRunning = false;
            this.IsComplete = true;
        }

        return true;
    }

    private bool AsExpected(out string message)
    {
        message = string.Empty;
        QuRegister lastRegister = this.Stages[^1].StageRegister;
        if ((this.ExpectedFinalProbabilities is not null) &&
            (this.ExpectedFinalProbabilities.Count == lastRegister.State.Count))
        {
            bool valid = true;
            List<Tuple<string, double>> bitValuesProbabilities = lastRegister.BitValuesProbabilities();
            for (int i = 0; i < bitValuesProbabilities.Count; i++)
            {
                if (!MathUtilities.AreAlmostEqual(
                    this.ExpectedFinalProbabilities[i], bitValuesProbabilities[i].Item2))
                {
                    message = "Run: Machine not providing expected probabilities.";
                    valid = false;
                }
            }

            if (!valid)
            {
                Debug.WriteLine("Run: Machine not providing expected probabilities.");
                for (int i = 0; i < bitValuesProbabilities.Count; i++)
                {
                    Debug.WriteLine(
                        string.Format(
                            "Bits: {0}, Expected: {1:F2}, Returned: {2:F2}",
                            bitValuesProbabilities[i].Item1,
                            this.ExpectedFinalProbabilities[i],
                            bitValuesProbabilities[i].Item2));
                }

                return false;
            }
        }
        else
        {
            Debug.WriteLine("Run: Machine not defining expected probabilities.");
        }

        return true;
    }

    #region   Do NOT Delete ~~~ Used for Unit Tests Serialization 

#pragma warning disable CA2211 // Non-constant fields should not be visible
    public static QuComputer Example =
#pragma warning restore CA2211 
        new()
        {
            Name = "Entanglement",
            Description = "Hadamard followed by CNot",
            QuBitsCount = 2,
            InitialStates = [QuState.Zero, QuState.Zero],
            Stages =
            [
                new QuStage()
                {
                   Operators =  [ new QuStageOperator { GateKey = "H" , QuBitIndices = [0]  }, ] ,
                },
                new QuStage()
                {
                   Operators = [ new QuStageOperator { GateKey = "CX" , QuBitIndices = [0,1]}, ] ,
                },
            ],
        };

    #endregion   Do NOT Delete ~~~ Used for Unit Tests Serialization 
}
