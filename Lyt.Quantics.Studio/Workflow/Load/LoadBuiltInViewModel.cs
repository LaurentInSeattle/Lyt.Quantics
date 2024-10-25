namespace Lyt.Quantics.Studio.Workflow.Load;

using static Lyt.Quantics.Engine.Utilities.SerializationUtilities; 

public sealed class LoadBuiltInViewModel : Bindable<LoadBuiltInView>
{
    public LoadBuiltInViewModel()  { }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        var computerNames = SerializationUtilities.GetEmbeddedComputerNames();
        List<BuiltInViewModel> list = new(computerNames.Count);
        foreach (string computerName in computerNames)
        {
            try
            {
                string resourceFileName = computerName ;
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

                list.Add(new BuiltInViewModel(computerName, computer )); 
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex);
                this.Logger.Warning( "Failed to load " +  ex.ToString() );
                continue ;
            }
        }

        this.BuiltInViews = list;
    }

    public List<BuiltInViewModel> BuiltInViews
    {
        get => this.Get<List<BuiltInViewModel>>()!;
        set => this.Set(value);
    }
}
