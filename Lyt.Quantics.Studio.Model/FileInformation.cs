namespace Lyt.Quantics.Studio.Model;

public record class FileInformation(string Name, string Description)
{
    // Must have an emply CTOR so that the UI validator can create it anew 
    public FileInformation() : this(string.Empty, string.Empty) { }
}
