namespace Lyt.Quantics.Studio.Model;

using Lyt.Quantics.Engine.Core;
using static FileManagerModel;

public sealed partial class QuanticsStudioModel : ModelBase
{
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

        // Create a 'blank' computer at initialization time, and two qubits to it 
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
            // FORNOW: Create a 'blank' computer at initialization time 
            this.QuComputer = new ("Untitled", "New quantum computer project."); 

            //if (!this.fileManager.Exists(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename))
            //{
            //    this.fileManager.Save(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename, TemplatesModel.DefaultTemplate);
            //}

            //TemplatesModel model =
            //    this.fileManager.Load<TemplatesModel>(Area.User, Kind.Json, TemplatesModel.TemplatesModelFilename);

            //// Copy all properties with attribute [JsonRequired]
            //base.CopyJSonRequiredProperties<TemplatesModel>(model);
            return Task.CompletedTask;
        }
        catch (Exception /* ex */ )
        {
            //string msg = "Failed to load TemplatesModel from " + TemplatesModel.TemplatesModelFilename;
            //this.Logger.Fatal(msg);
            //throw new Exception(msg, ex);
            throw ;
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

    public static List<Gate> Gates 
    {  
        get
        {
            var gateTypes = GateFactory.AvailableProducts;
            var list = new List<Gate>(gateTypes.Count);
            foreach (var gateType in gateTypes)
            {
                var gate = GateFactory.Produce(gateType.Key); 
                list.Add(gate);
            } 

            return list;
        }
    }
}
