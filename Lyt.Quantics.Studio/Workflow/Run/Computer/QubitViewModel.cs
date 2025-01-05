﻿namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

public sealed class QubitViewModel : Bindable<QubitView>
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
    private const string ket = "\u276D";

    private static readonly VisualState OnVisualState;

    private static readonly VisualState OffVisualState;

    static QubitViewModel()
    {
        if ((Utilities.TryFindResource("ToggleOnGeneralVisualState", out VisualState? maybeOnVisualState)) &&
            (Utilities.TryFindResource("ToggleOffGeneralVisualState", out VisualState? maybeOffVisualState)) &&
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
    private QuState quState;

    public QubitViewModel(int qubitIndex)
    {
        this.qubitIndex = qubitIndex;
        this.quanticsStudioModel = App.GetRequiredService<QsModel>();

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

    protected override void OnViewLoaded()
    {
        this.Name = string.Concat("q", verySmallSpace, subscripts[this.qubitIndex]);
        this.RefreshKet();
    }

#pragma warning disable IDE0051 // Remove unused private members
    // Autogenerated bindings

    private void OnKet(object? _)
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

    private void OnMeasure(object? _)
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

#pragma warning restore IDE0051 

    private void RefreshKet()
        => this.Ket =
            string.Concat(bar, smallSpace, this.quState.ToUiString(), smallSpace, ket);

    public ICommand KetCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public ICommand MeasureCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Ket { get => this.Get<string>()!; set => this.Set(value); }

    public VisualState VisualState
    {
        get => this.Get<VisualState>()!;
        set => this.Set(value);
    }
}
