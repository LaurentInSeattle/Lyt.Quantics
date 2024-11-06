namespace Lyt.Quantics.Studio.Workflow.Run.Amplitudes;

using static ToolbarCommandMessage;
using static MessagingExtensions;

public sealed class AmplitudesToolbarViewModel : Bindable<AmplitudesToolbarView>
{
    private readonly QsModel quanticsStudioModel;

    private bool isInitializing;

    public AmplitudesToolbarViewModel()
    {
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
        this.Messenger.Subscribe<ModelStructureUpdateMessage>(this.OnModelStructureUpdateMessage);
    }

    private void OnModelStructureUpdateMessage(ModelStructureUpdateMessage message)
    {
        this.isInitializing = true;
        var computer = this.quanticsStudioModel.QuComputer;
        this.StageCount = computer.Stages.Count;
        this.StageRank = computer.Stages.Count;
        this.isInitializing = false;
    }

    protected override void OnViewLoaded()
    {
        this.ShowAll = true;
        this.ShowByBitOrder = true;
    }

    public bool ShowAll
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            Command(ToolbarCommand.ShowAll, value);
        }
    }

    public bool ShowByBitOrder
    {
        get => this.Get<bool>();
        set
        {
            this.Set(value);
            Command(ToolbarCommand.ShowByBitOrder, value);
        }
    }

    public double StageCount { get => this.Get<double>(); set => this.Set(value); }

    public double StageRank
    {
        get => this.Get<double>();
        set
        {
            this.Set(value);
            int stageRank = (int)Math.Round(this.StageRank);
            this.StageRankText = string.Format("Stage {0:D}", stageRank);
            if (!this.isInitializing)
            {
                Command(ToolbarCommand.ShowStage, stageRank);
            }
        }
    }

    public string StageRankText { get => this.Get<string>()!; set => this.Set(value); }
}
