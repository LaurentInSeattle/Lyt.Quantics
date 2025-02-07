namespace Lyt.Quantics.Engine.Machine;

using MathNet.Numerics.LinearAlgebra;
using System;

public sealed partial class QuComputer
{
    private const string DefaultName = "< Untitled >";
    private const string DefaultDescription = "< Undocumented >";
    private const string DefaultComment = "< No comments >";

    private bool isValid;
    private bool isBuilt;
    private bool isPrepared;
    private bool isRunning;
    private bool isComplete;

    private CancellationTokenSource? cancellationTokenSource;
    private Action<bool, int>? updateDelegate; 

    public QuComputer() { /* Required for deserialization */ }

    public QuComputer(string name, string description)
    {
        this.Name = name;
        this.Description = description;
    }

    #region JSON Properties (serialized)

    public DateTime Created { get; set; } = DateTime.Now;

    public DateTime LastOpened { get; set; } = DateTime.Now;

    public DateTime LastModified { get; set; } = DateTime.Now;

    public bool IsUnitTest { get; set; } = false;

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
    public KetMap KetMap { get; private set; } = new(1);

    [JsonIgnore]
    public QuRegister InitialRegister { get; private set; } = new(1);

    [JsonIgnore]
    public QuRegister FinalRegister { get; private set; } = new(1);

    [JsonIgnore]
    public Vector<float> Result { get; private set; } = Vector<float>.Build.Dense(1);

    [JsonIgnore]
    public bool RunUsingKroneckerProduct { get; set; }

    #region Machine states 

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
            if (!value)
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
                this.IsRunning = false;
                this.IsComplete = false;
            }
        }
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

    #endregion Machine states 

    [JsonIgnore]
    public int StepIndex { get; private set; }

    #endregion Runtime Properties not serialized

    public QuComputer DeepClone()
    {
        var clone = this.CreateAndCopyPropertiesFrom();
        foreach (var state in this.InitialStates)
        {
            clone.InitialStates.Add(state);
        }

        foreach (var stage in this.Stages)
        {
            clone.Stages.Add(stage.DeepClone());
        }

        return clone;
    }

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

        if (this.QuBitsCount > QuRegister.MaxQubits)
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
            this.KetMap = new KetMap(this.QuBitsCount);

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
            //Debug.WriteLine(string.Format("Initial State: {0}", this.InitialRegister.State));
            //Debug.WriteLine(
            //    string.Format(
            //        "Initial State Probabilities: {0}",
            //        Vector<double>.Build.Dense([.. this.InitialRegister.KetProbabilities()])));
        }
        catch (Exception ex)
        {
            message = string.Concat("Prepare: Exception thrown: " + ex.Message);
            return false;
        }

        this.IsPrepared = true;
        return true;
    }

    public void Randomize() => this.InitialRegister.Randomize();

    public void Initialize(QuRegister initialState) => this.InitialRegister = initialState;

    public bool Reset(out string message)
    {
        this.IsValid = false;
        for (int qubitIndex = 0; qubitIndex < this.QuBitsCount; ++qubitIndex)
        {
            this.InitialStates[qubitIndex] = QuState.Zero;
        }

        return this.Validate(out message);
    }

    /// <summary> Non threaded version of running the machine, for Unit Tests only </summary>
    public bool Run(bool checkExpected, Action<bool, int>? onUpdate, out string message)
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

        try
        {
            this.updateDelegate = onUpdate;
            Tuple<bool, string>? tuple = this.RunInternal(checkExpected, withCTS: false);
            if (tuple is not null)
            {
                message = tuple.Item2;
                return tuple.Item1;
            }
            else
            {
                message = "Unexpected error: null return";
                Debug.WriteLine(message);
                return false;
            }
        }
        catch (OperationCanceledException e)
        {
            Debug.WriteLine("Cancelled " + e.Message);
            message = "Cancelled";
        }
        catch (Exception e)
        {
            Debug.WriteLine("Exception thrown: " + e.Message);
            message = "Exception thrown: " + e.Message;
            return false;
        }

        return true;
    }

    /// <summary> Breaks a UI Run of the machine, awaitable </summary>
    public async Task<Tuple<bool, string>> Break ()
    {
        string message = string.Empty;
        if (!this.IsRunning)
        {
            message = "Run: Machine is not running.";
            return new Tuple<bool, string>(false, message);
        }

        if (this.cancellationTokenSource is null)
        {
            message = "Run: Failed to cancel the Machine Run.";
            return new Tuple<bool, string>(false, message);
        }

        this.updateDelegate?.Invoke(true, this.StepIndex - 1);
        this.cancellationTokenSource.Cancel();

        await Task.Delay(200); 

        // TODO: Check stopped 

        return new Tuple<bool, string>(true, message);
    }

    /// <summary> Launch a UI Run of the machine, awaitable </summary>
    public async Task<Tuple<bool, string>> Run(bool checkExpected, Action<bool, int>? onUpdate)
    {
        string message = string.Empty;
        if (this.IsRunning)
        {
            message = "Run: Machine is already running.";
            return new Tuple<bool, string>(false, message);
        }

        if (this.IsComplete)
        {
            message = "Run: Machine is in Complete State: Invoke Prepare before running";
            return new Tuple<bool, string>(false, message);
        }

        if (!this.IsPrepared)
        {
            message = "Run: Machine has not been prepared: Invoke Prepare before running";
            return new Tuple<bool, string>(false, message);
        }

        this.cancellationTokenSource = new();

        try
        {
            this.updateDelegate = onUpdate; 
            this.IsRunning = true;
            Tuple<bool, string>? tuple = null ;             
            await Task.Run(()=> 
            {
                tuple = this.RunInternal(checkExpected);
            }, this.cancellationTokenSource.Token);

            if (tuple is not null)
            {
                return tuple;
            } 
            else
            {
                message = "Unexpected error: null return";
                Debug.WriteLine(message);
                return new Tuple<bool, string>(false, message);
            }
        }
        catch (OperationCanceledException e)
        {
            Debug.WriteLine("Cancelled " + e.Message);
            message = "Cancelled";
        }
        catch (Exception e)
        {
            Debug.WriteLine("Exception thrown: " + e.Message);
            message = "Exception thrown: " + e.Message;
            return new Tuple<bool, string>(false, message);
        }
        finally
        {
            this.cancellationTokenSource.Dispose();
            this.cancellationTokenSource = null ;
        }

        return new Tuple<bool, string>(true, message); 
    }

    /// <summary> Run the machine, used by both UI or UT 'worlds' </summary>
    private Tuple<bool, string> RunInternal(bool checkExpected, bool withCTS = true)
    {
        bool Step(out string message)
        {
            try
            {
                // Single Step
                var stage = this.Stages[this.StepIndex];
                Debug.WriteLine(string.Format("Step: {0}  {1}", this.StepIndex, stage.Operations));
                QuRegister sourceRegister =
                    this.StepIndex == 0 ? this.InitialRegister : this.Stages[this.StepIndex - 1].StageRegister;
                stage.Calculate(this, sourceRegister, out message);
                if (!string.IsNullOrEmpty(message))
                {
                    Debug.WriteLine(message);
                    return false;
                }

                //Debug.WriteLine(
                //    string.Format("Step: {0} - Probabilities: {1}", this.StepIndex, stage.KetProbabilities));
            }
            catch (Exception ex)
            {
                message = string.Concat("Step: Exception thrown: " + ex.Message);
                return false;
            }

            return true;
        }

        string message;
        try
        {
            if (withCTS)
            {
                if (this.cancellationTokenSource is null)
                {
                    return new Tuple<bool, string>(false, "Run: Cannot run: No CTS.");
                }

                this.cancellationTokenSource.Token.ThrowIfCancellationRequested();
            }

            // running the stages 
            for (int i = 0; i < this.Stages.Count; i++)
            {
                if (Step(out message))
                {
                    ++this.StepIndex;
                    this.updateDelegate?.Invoke(false, this.StepIndex);

                    if ( withCTS)
                    {
                        if (this.cancellationTokenSource is null)
                        {
                            return new Tuple<bool, string>(false, "Run: Cannot run: No CTS.");
                        }

                        this.cancellationTokenSource?.Token.ThrowIfCancellationRequested();
                    }
                }
                else
                {
                    return new Tuple<bool, string>(false, message);
                }
            }

            if (checkExpected && (!this.AsExpected(out message)))
            {
                return new Tuple<bool, string>(false, message);
            }

            // Measure last register
            QuRegister lastRegister = this.Stages[^1].StageRegister;
            this.FinalRegister = lastRegister.DeepClone();
            Vector<float> measure = Vector<float>.Build.Dense([.. lastRegister.Measure()]);

            Debug.WriteLine("Last stage, last register: " + lastRegister.ToString());
            Debug.WriteLine("Last stage, measure: " + measure.ToString());
            this.Result = measure;
        }
        catch (Exception ex)
        {
            message = string.Concat("Step: Exception thrown: " + ex.Message);
            return new Tuple<bool, string>(false, message);  
        }
        finally
        {
            this.IsRunning = false;
            this.IsComplete = true;
            this.updateDelegate?.Invoke(true, this.StepIndex);
        }

        return new Tuple<bool, string>(true, string.Empty) ;
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
                   Operators =  [ new QuStageOperator( "H") { TargetQuBitIndices = [0]  }, ] ,
                },
                new QuStage()
                {
                   Operators =
                   [
                       new QuStageOperator ("CX" ) { ControlQuBitIndices = [0], TargetQuBitIndices = [1]},
                   ] ,
                },
            ],
        };

    #endregion   Do NOT Delete ~~~ Used for Unit Tests Serialization 
}
