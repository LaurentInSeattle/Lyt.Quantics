namespace Lyt.Quantics.Studio.Messaging;

public sealed class QubitChangedMessage ( int index , QuState quState )
{
    public int Index { get; set; } = index;

    public QuState InitialState{ get; set; } = quState;
}
