namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

using static ToolbarCommandMessage;
using static ApplicationMessagingExtensions;

public sealed partial class AmplitudesToolbarViewModel :
    ViewModel<AmplitudesToolbarView>, IRecipient<ModelStructureUpdateMessage>
{
    private readonly QsModel quanticsStudioModel;

    [ObservableProperty]
    private bool showAll;

    [ObservableProperty]
    private bool showByBitOrder;

    [ObservableProperty]
    private double stageCount;

    [ObservableProperty]
    private double stageRank;

    [ObservableProperty]
    private string stageRankText;

    private bool isInitializing;

    public AmplitudesToolbarViewModel()
    {
        this.stageRankText = string.Empty;
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
        this.Subscribe<ModelStructureUpdateMessage>();
    }

    public void Receive(ModelStructureUpdateMessage message)
    {
        this.isInitializing = true;
        var computer = this.quanticsStudioModel.QuComputer;
        this.StageCount = computer.Stages.Count;
        this.StageRank = computer.Stages.Count;
        this.isInitializing = false;
    }

    public override void OnViewLoaded()
    {
        this.ShowAll = true;
        this.ShowByBitOrder = true;
    }

    partial void OnShowAllChanged(bool value) => Command(ToolbarCommand.ShowAll, value);

    partial void OnShowByBitOrderChanged(bool value) => Command(ToolbarCommand.ShowByBitOrder, value);

    partial void OnStageRankChanged(double value)
    {
        int rank = (int)Math.Round(this.StageRank);
        this.StageRankText = string.Format("Stage {0:D}", rank);
        if (!this.isInitializing)
        {
            Command(ToolbarCommand.ShowStage, rank);
        }
    }
}
