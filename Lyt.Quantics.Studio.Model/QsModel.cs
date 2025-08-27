namespace Lyt.Quantics.Studio.Model;

using Lyt.Quantics.Studio.Model.Algoritms.ShorFactorisation;
using static FileManagerModel;

public sealed partial class QsModel : ModelBase
{
    public static Dictionary<string, QuComputer> BuiltInComputers { get; private set; } = [];

    public static List<Gate> Gates { get; private set; } = [];

    private readonly FileManagerModel fileManager;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable IDE0021 // Use expression body for constructor 
    public QsModel() : base(null)
    {
        // Do not inject the FileManagerModel instance: a parameter-less ctor is required for Deserialization 
        // Empty CTOR required for deserialization 
        this.ShouldAutoSave = false;

    }
#pragma warning restore IDE0021
#pragma warning restore CS8625 
#pragma warning restore CS8618

    public QsModel(FileManagerModel fileManager, ILogger logger) : base(logger)
    {
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;

        this.Projects = []; 

        // Create a 'blank' computer at initialization time, and nothing in it 
        this.QuComputer = new("Untitled", "New quantum computer project.");
        this.ShowBuiltInComputers = true;
        this.ShowRecentDocuments = true;

        ShorClassic.Poke(); 
    }

    public Dictionary<string, QuComputer> Projects { get; private set; }

    public override async Task Initialize() => await this.Load();

    public override async Task Shutdown()
    {
        if (this.IsDirty)
        {
            await this.Save();
        }
    }

    public Task Load()
    {
        try
        {
            // This needs to complete BEFORE we reach the Load page,
            // so do NOT include in the "Fire and forget" thread below 
            this.EnumerateProjects(); 

            // Fire and forget 
            Task.Run(() =>
            {
                LoadGates();
                LoadBuiltInComputers(this.Logger);
            } );

            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            this.Logger.Fatal(ex.Message);
            throw;
        }
    }

    public override Task Save() => Task.CompletedTask;
    // Null check is needed !
    // If the File Manager is null we are currently loading the model and activating properties on a second instance 
    // causing dirtyness, and in such case we must avoid the null crash and anyway there is no need to save anything.
    //if (this.fileManager is not null)
    //{
    //    this.fileManager.Save(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename, this);
    //    base.Save();
    //}        

    private static void LoadGates()
    {
        var gateTypesDictionary = GateFactory.AvailableProducts;
        var list = new List<Gate>(gateTypesDictionary.Count);
        foreach (var gateType in gateTypesDictionary)
        {
            // Because of missing axis in default gate parameters, we need to create these 
            // outside this loop 
            if ( gateType.Value == typeof(RotationGate))
            {
                continue ;
            }

            if (gateType.Value == typeof(ControlledGate))
            {
                continue;
            }

            // Phase gate (no axis) will also be created with defaults in the tool box
            var gate = GateFactory.Produce(gateType.Key, new GateParameters());
            list.Add(gate);
        }

        // Add all three rotation gates with a Pi / 2 angle 
        GateParameters defaultGateParameters = new();
        foreach (Axis axis in new Axis[] { Axis.X, Axis.Y, Axis.Z })
        {
            defaultGateParameters.Axis = axis;
            list.Add(new RotationGate(defaultGateParameters));
        }

        // Add a Controlled Hadamard gate 
        list.Add (new ControlledGate(new HadamardGate()));

        Gates = list;
    }

    private static void LoadBuiltInComputers(ILogger logger)
    {
        var computerNames = SerializationUtilities.GetEmbeddedComputerNames();
        Dictionary<string, QuComputer> dictionary = new(computerNames.Count);
        foreach (string computerName in computerNames)
        {
            try
            {
                string resourceFileName = computerName;
                if (!resourceFileName.EndsWith(SerializationUtilities.ResourcesExtension))
                {
                    resourceFileName = computerName + SerializationUtilities.ResourcesExtension;
                }

                string serialized = SerializationUtilities.LoadEmbeddedTextResource(resourceFileName, out string? resourceFullName);
                var computer = SerializationUtilities.Deserialize<QuComputer>(serialized) ?? throw new Exception("Failed to deserialize");
                bool isValid = computer.Validate(out string message);
                if (!isValid)
                {
                    throw new Exception(message);
                }

                if (!string.IsNullOrEmpty(resourceFullName))
                {
                    if (resourceFullName.Contains("Test"))
                    {
                        computer.IsUnitTest = true;
                    }
                }

                bool isBuilt = computer.Build(out message);
                if (!isBuilt)
                {
                    throw new Exception(message);
                }

                dictionary.Add(computer.Name, computer);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                logger.Warning("LoadBuiltInComputers: Failed to load " + ex.ToString());

                continue;
            }
        }

        BuiltInComputers = dictionary;
    }

    private void EnumerateProjects ()
    {
        try
        {
            this.Projects.Clear();
            var files = this.fileManager.Enumerate(Area.User, Kind.Json);
            foreach (string file in files)
            {
                try
                {
                    var computer = this.fileManager.Load<QuComputer>(Area.User, Kind.Json, file) ?? throw new Exception("Failed to deserialize");
                    bool isValid = computer.Validate(out string message);
                    if (!isValid)
                    {
                        throw new Exception(message);
                    }

                    bool isBuilt = computer.Build(out message);
                    if (!isBuilt)
                    {
                        throw new Exception(message);
                    }

                    this.Projects.Add(file, computer);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    this.Logger.Warning(file + " :  failed to load \n" + ex.ToString());
                    continue;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            this.Logger.Warning("One or more files failed to load \n" + ex.ToString());
        }
    }
}
