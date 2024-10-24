namespace Lyt.Quantics.Studio.Workflow.Run.Computer; 

public sealed class ComputerActivationParameter (
    ComputerActivationParameter.Kind activationKind, string path = "")
{
    public enum Kind
    {
        New, 
        Resource, 
        Document,
    }

    public Kind ActivationKind { get; private set; } = activationKind;

    public string Path { get; private set; } = path;
}
