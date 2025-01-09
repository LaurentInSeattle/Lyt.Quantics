namespace Lyt.Quantics.Studio.Workflow.Run.Gates;

public abstract class GateViewModelBase<TView> 
    : Bindable<TView>, IDraggableBindable, IGateInfoProvider
    where TView : Control, new()
{
    public const string CustomDragAndDropFormat = "GateViewModel";

    protected readonly QsModel quanticsStudioModel;
    protected readonly IToaster toaster;

    public GateViewModelBase(
        Gate gate, QubitsIndices qubitsIndices, 
        bool isGhost , bool isToolBox, int stageIndex)
    {
        // Too many properties here and too many gates !
        this.DisablePropertyChangedLogging = true;

        // Do not use Injection directly as this is loaded programmatically by the RunView 
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();
        this.toaster = App.GetRequiredService<IToaster>();

        this.Gate = gate;
        this.QubitsIndices = qubitsIndices;
        this.IsGhost = isGhost;
        this.IsToolbox = isToolBox;
        this.StageIndex = stageIndex;
    }

    #region IGateInfoProvider Implementation 

    public Gate Gate { get; private set; }

    /// <summary> Only valid when this is NOT a toolbox gate view model. </summary>
    public QubitsIndices QubitsIndices { get; private set; }

    /// <summary> True when this is a ghost gate view model. </summary>
    public bool IsGhost { get; private set; }

    /// <summary> True when this is a toolbox gate view model. </summary>
    public bool IsToolbox { get; private set; }

    /// <summary> Only valid when this is NOT a toolbox gate view model. </summary>
    public int StageIndex { get; private set; }

    #endregion IGateInfoProvider Implementation 

    #region IDraggableBindable Implementation 

    public Draggable? Draggable { get; protected set; }

    public string DragDropFormat => GateViewModelBase<TView>.CustomDragAndDropFormat;

    public void OnEntered()
        => this.Messenger.Publish(
            new GateHoverMessage(IsEnter: true, this.Gate.CaptionKey, this.Gate.Description));

    public void OnExited() => this.Messenger.Publish(new GateHoverMessage());

    public void OnLongPress()
    {
        if (this.IsGhost || this.IsToolbox)
        {
            return;
        }

        this.Remove();
    }

    public void OnClicked(bool isRightClick)
    {
        if (this.IsToolbox)
        {
            return;
        }

        if (this.Gate.IsEditable)
        {
            // Launch edit gate dialog 
            this.Messenger.Publish(new GateEditMessage(this, isRightClick));
        }
    }

    public bool OnBeginDrag() => true;

    public abstract UserControl CreateGhostView();

    #endregion IDraggableBindable Implementation 

    private void Remove()
    {
        Debug.WriteLine("Removing gate: " + this.Gate.CaptionKey);
        if (!this.quanticsStudioModel.RemoveGate(
            this.StageIndex, this.QubitsIndices, this.Gate, out string message))
        {
            this.toaster.Show(
                "Failed to Remove gate: " + this.Gate.CaptionKey, message,
                4_000, InformationLevel.Error);
        }
    }
}
