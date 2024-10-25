namespace Lyt.Quantics.Studio.Workflow.Load;

public sealed class LoadBuiltInViewModel : Bindable<LoadBuiltInView>
{
    private readonly QuanticsStudioModel quanticsStudioModel;

    public LoadBuiltInViewModel()  
    {
        // Do not use Injection directly as this is loaded programmatically by the LoadView 
        this.quanticsStudioModel = App.GetRequiredService<QuanticsStudioModel>();
    }

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();

        var builtInComputers = QuanticsStudioModel.BuiltInComputers;
        List<BuiltInViewModel> builtInViews = new(builtInComputers.Count);
        foreach (string computerName in builtInComputers.Keys)
        {
            try
            {
                var computer = builtInComputers[computerName];
                builtInViews.Add(new BuiltInViewModel(computerName, computer )); 
            }
            catch (Exception ex) 
            {
                Debug.WriteLine(ex);
                this.Logger.Warning( "Failed to load " +  ex.ToString() );
                continue ;
            }
        }

        this.BuiltInViews = builtInViews;
    }

    public List<BuiltInViewModel> BuiltInViews
    {
        get => this.Get<List<BuiltInViewModel>>()!;
        set => this.Set(value);
    }
}
