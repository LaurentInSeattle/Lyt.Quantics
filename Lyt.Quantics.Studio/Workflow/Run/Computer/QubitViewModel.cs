namespace Lyt.Quantics.Studio.Workflow.Run.Computer;

using static ToolbarCommandMessage;

public sealed class QubitViewModel : Bindable<QubitView>
{
    // See: https://en.wikipedia.org/wiki/Unicode_subscripts_and_superscripts
    // And: https://www.unicode.org/Public/UNIDATA/ 
    private static readonly string[] subscripts =
    {
        "\u2080","\u2081","\u2082","\u2083","\u2084",
        "\u2085","\u2086","\u2087","\u2088","\u2089",
    };

    private const string smallSpace = "\u2009";
    private const string bar = "\u2223";
    private const string bra = "\u276C";
    private const string ket = "\u276D";

    private readonly int qubitIndex;

    public QubitViewModel(int qubitIndex)
    {
        this.qubitIndex = qubitIndex;
    }

    protected override void OnViewLoaded()
    {
        this.Name = string.Concat("q", smallSpace , subscripts[this.qubitIndex]);
        this.Ket = string.Concat(bar , smallSpace, "0" , smallSpace, ket); 
    }

    private void OnKet(object? _) { }

    public ICommand KetCommand { get => this.Get<ICommand>()!; set => this.Set(value); }

    public string Name { get => this.Get<string>()!; set => this.Set(value); }

    public string Ket { get => this.Get<string>()!; set => this.Set(value); }
}
