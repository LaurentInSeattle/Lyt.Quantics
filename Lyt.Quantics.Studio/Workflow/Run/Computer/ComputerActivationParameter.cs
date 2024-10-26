namespace Lyt.Quantics.Studio.Workflow.Run.Computer; 

public sealed record class ComputerActivationParameter (
    ComputerActivationParameter.Kind ActivationKind, string Name = "", string Path = "")
{
    public enum Kind
    {
        New, 
        Resource, 
        Document,
    }
}
