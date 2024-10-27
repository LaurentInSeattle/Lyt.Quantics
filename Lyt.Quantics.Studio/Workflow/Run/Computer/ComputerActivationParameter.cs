namespace Lyt.Quantics.Studio.Workflow.Run.Computer; 

public sealed record class ComputerActivationParameter (
    ComputerActivationParameter.Kind ActivationKind, 
    string Name = "", 
    QuComputer? QuComputer = null)
{
    public enum Kind
    {
        New, 
        Resource, 
        Document,
        Back, // From Save
    }
}
