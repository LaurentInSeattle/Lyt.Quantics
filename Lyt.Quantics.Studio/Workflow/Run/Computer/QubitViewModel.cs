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

    private readonly int qubitIndex;
    private QuState quState;

    public QubitViewModel(int qubitIndex)
    {
        this.qubitIndex = qubitIndex;
        this.quState = QuState.Zero;
        this.Messenger.Subscribe<ModelResetMessage>(this.OnModelResetMessage);
    }

    private void OnModelResetMessage(ModelResetMessage _)
    {
        this.quState = QuState.Zero;
        this.RefreshKet();
    } 

    protected override void OnViewLoaded()
    {
        this.Name = string.Concat("q", verySmallSpace, subscripts[this.qubitIndex]);
        this.RefreshKet() ;
    }

#pragma warning disable IDE0051 // Remove unused private members
    // Autogenerated binding
    private void OnKet(object? _)
#pragma warning restore IDE0051 
    {
        int maxStates = Enum.GetValues(typeof(QuState)).Length;
        int index = 1 + (int)this.quState;
        if (index >= maxStates)
        {
            index = 0;
        }

        this.quState = (QuState)index;
        this.RefreshKet();
        this.Messenger.Publish(new QubitChangedMessage(this.qubitIndex, this.quState));
    }

    private void RefreshKet()
        => this.Ket = 
            string.Concat(bar, smallSpace, this.quState.ToUiString(), smallSpace, ket);

    public ICommand KetCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Ket { get => this.Get<string>()!; set => this.Set(value); }
}
