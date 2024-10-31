namespace Lyt.Quantics.Studio.Workflow.Load;

public sealed class LoadBuiltInViewModel : Bindable<LoadBuiltInView>
{
    private List<BuiltInViewModel> builtInViews = [];

    protected override void OnViewLoaded()
    {
        base.OnViewLoaded();
        this.BuiltInViews = builtInViews;
    }

    public override void Activate(object? activationParameters)
    {
        base.Activate(activationParameters);

        var builtInComputers = QuanticsStudioModel.BuiltInComputers;
        this.builtInViews = new(builtInComputers.Count);
        foreach (string computerName in builtInComputers.Keys)
        {
            try
            {
                var computer = builtInComputers[computerName];
                builtInViews.Add(new BuiltInViewModel(computer));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                this.Logger.Warning("Failed to load " + ex.ToString());
                continue;
            }
        }
    }

    public List<BuiltInViewModel> BuiltInViews
    {
        get => this.Get<List<BuiltInViewModel>>()!;
        set => this.Set(value);
    }
}
