namespace Lyt.Quantics.Studio.Model;

using static FileManagerModel;

public sealed partial class QuanticsStudioModel : ModelBase
{
    public const int MaxQubits = 10; // For now ~ 10 could be doable ? 

    private readonly FileManagerModel fileManager;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable IDE0021 // Use expression body for constructor 
    public QuanticsStudioModel() : base(null, null)
    {
        // Do not inject the FileManagerModel instance: a parameter-less ctor is required for Deserialization 
        // Empty CTOR required for deserialization 
        this.ShouldAutoSave = false;

    }
#pragma warning restore IDE0021
#pragma warning restore CS8625 
#pragma warning restore CS8618

    public QuanticsStudioModel(FileManagerModel fileManager, IMessenger messenger, ILogger logger) : base(messenger, logger)
    {
        this.fileManager = fileManager;
        this.ShouldAutoSave = true;

        // Create a 'blank' computer at initialization time, and nothing in it 
        this.QuComputer = new("Untitled", "New quantum computer project.");
    }

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
            //if (!this.fileManager.Exists(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename))
            //{
            //    this.fileManager.Save(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename, TemplatesModel.DefaultTemplate);
            //}

            //TemplatesModel model =
            //    this.fileManager.Load<TemplatesModel>(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename);

            //// Copy all properties with attribute [JsonRequired]
            //base.CopyJSonRequiredProperties<TemplatesModel>(model);

            // Fire and forget 
            Task.Run(() =>
            {
                LoadGates();
                LoadBuiltInComputers(this.Logger);
            } );

            return Task.CompletedTask;
        }
        catch (Exception /* ex */ )
        {
            //string msg = "Failed to load TemplatesModel from " + TemplatesModel.TemplatesModelFilename;
            //this.Logger.Fatal(msg);
            //throw new Exception(msg, ex);
            throw;
        }
    }

    public override Task Save()
    {
        // Null check is needed !
        // If the File Manager is null we are currently loading the model and activating properties on a second instance 
        // causing dirtyness, and in such case we must avoid the null crash and anyway there is no need to save anything.
        //if (this.fileManager is not null)
        //{
        //    this.fileManager.Save(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename, this);
        //    base.Save();
        //}

        return Task.CompletedTask;
    }

    public static Dictionary<string, QuComputer> BuiltInComputers { get; private set; } = [];

    public static List<Gate> Gates { get; private set; } = [];

    private static void LoadGates()
    {
        var gateTypes = GateFactory.AvailableProducts;
        var list = new List<Gate>(gateTypes.Count);
        foreach (var gateType in gateTypes)
        {
            var gate = GateFactory.Produce(gateType.Key);
            list.Add(gate);
        }

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

                string serialized = SerializationUtilities.LoadEmbeddedTextResource(resourceFileName);
                var computer = SerializationUtilities.Deserialize<QuComputer>(serialized);
                if (computer is null)
                {
                    throw new Exception("Failed to deserialize");
                }

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

                dictionary.Add(computerName, computer);
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
}
