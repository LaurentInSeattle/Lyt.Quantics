namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static Lyt.Avalonia.Controls.Utilities;

public sealed partial class QubitViewModel : ViewModel<QubitView>
{
    // See: https://en.wikipedia.org/wiki/Unicode_subscripts_and_superscripts
    // And: https://www.unicode.org/Public/UNIDATA/ 
    private static readonly string[] subscripts =
    [
        "\u2080","\u2081","\u2082","\u2083","\u2084",
        "\u2085","\u2086","\u2087","\u2088","\u2089",
    ];

    private const string smallSpace = "\u2009";
    private const string verySmallSpace = "\u2009";
    private const string bar = "\u2223";
    // private const string bra = "\u276C";
    private const string ketString = "\u276D";

    private static readonly VisualState OnVisualState;

    private static readonly VisualState OffVisualState;

    static QubitViewModel()
    {
        if ((TryFindResource("ToggleOnGeneralVisualState", out VisualState? maybeOnVisualState)) &&
            (TryFindResource("ToggleOffGeneralVisualState", out VisualState? maybeOffVisualState)) &&
            ((maybeOnVisualState is not null) &&
            (maybeOffVisualState is not null)))
        {
            OnVisualState = maybeOnVisualState;
            OffVisualState = maybeOffVisualState;
        }
        else
        {
            throw new Exception("Missing required resources");
        }
    }

    private readonly QsModel quanticsStudioModel;
    private readonly int qubitIndex;

    [ObservableProperty]
    private string name;

    [ObservableProperty]
    private string ket;

    [ObservableProperty]
    private VisualState visualState;

    private QuState quState;

    public QubitViewModel(int qubitIndex)
    {
        this.qubitIndex = qubitIndex;
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();

        this.name = string.Empty;
        this.ket = string.Empty;
        this.quState = QuState.Zero;
        this.Messenger.Subscribe<ModelResetMessage>(this.OnModelResetMessage);
        this.IsSelected = true;
        this.VisualState = OnVisualState;
    }

    public bool IsSelected { get; private set; }

    private void OnModelResetMessage(ModelResetMessage _)
    {
        this.quState = QuState.Zero;
        this.RefreshKet();
    }

    public override void OnViewLoaded()
    {
        string subscript;
        if (this.qubitIndex < 10) 
        {
            subscript = subscripts[this.qubitIndex];
        }
        else
        {
            int tens = this.qubitIndex / 10;
            int units = this.qubitIndex % 10;
            subscript = string.Concat ( subscripts[tens], subscripts[units]);
        }

        this.Name = string.Concat("q", verySmallSpace, subscript);
        this.RefreshKet();
    }

    [RelayCommand]
    public void OnKet()
    {
        var keyboard = App.GetRequiredService<Keyboard>();
        bool isShifted = keyboard.Modifiers.HasFlag(KeyModifiers.Shift);

        int index;
        int maxStates = Enum.GetValues<QuState>().Length;
        if (isShifted)
        {
            index = (int)this.quState - 1;
            if (index < 0)
            {
                index = maxStates - 1;
            }
        }
        else
        {

            index = 1 + (int)this.quState;
            if (index >= maxStates)
            {
                index = 0;
            }
        }

        this.quState = (QuState)index;
        this.RefreshKet();
        this.Messenger.Publish(new QubitChangedMessage(this.qubitIndex, this.quState));
    }

    [RelayCommand]
    public void OnMeasure()
    {
        var toaster = App.GetRequiredService<IToaster>();
        this.IsSelected = !this.IsSelected;
        this.VisualState = this.IsSelected ? OnVisualState : OffVisualState;
        if ( ! this.quanticsStudioModel.UpdateQubitMeasureState(this.qubitIndex, this.IsSelected, out string message))
        {
            this.Logger.Warning(message);
            toaster.Show("Unexpected Error", message, 5_000, InformationLevel.Error);
        }

        if ( this.quanticsStudioModel.ShouldMeasureNoQubits)
        {
            toaster.Show("No Measures", "Amplitude view will remain empty.", 3_000, InformationLevel.Warning);
        }
    }

    private void RefreshKet()
        => this.Ket =
            string.Concat(bar, smallSpace, this.quState.ToUiString(), smallSpace, ketString);
}
