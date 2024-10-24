namespace Lyt.Quantics.Studio.Messaging;

public sealed class ShowTitleBarMessage(bool show = true)
{
    public bool Show { get; private set; } = show;
}
