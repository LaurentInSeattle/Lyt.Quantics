namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ToolbarCommandMessage;
using static ApplicationMessagingExtensions;

public sealed partial class ComputerToolbarViewModel : ViewModel<ComputerToolbarView>
{
    [ObservableProperty]
    private bool hideProbabilities;

#pragma warning disable CA1822 // Mark members as static

    [RelayCommand]
    public void OnAddQubit() => Command(ToolbarCommand.AddQubit);

    [RelayCommand]
    public void OnRemoveQubit() => Command(ToolbarCommand.RemoveQubit);

    [RelayCommand]
    public void OnPackStages() => Command(ToolbarCommand.PackStages);

    [RelayCommand]
    public void OnReset() => Command(ToolbarCommand.Reset);

    [RelayCommand]
    public void OnRun() => Command(ToolbarCommand.Run);

    [RelayCommand]
    public void OnSave() => Command(ToolbarCommand.Save);

    [RelayCommand]
    public void OnClose() => Command(ToolbarCommand.Close);

    partial void OnHideProbabilitiesChanged( bool value )
        => Command(ToolbarCommand.HideProbabilities, value);

#pragma warning restore CA1822 // Mark members as static
}
