namespace Lyt.Quantics.Engine.Machine; 

public sealed class QuComputer
{
    private readonly List<QuStep> steps; 

    public QuComputer(int quBitsCount)
    {
        if ( ( quBitsCount < 0) || ( quBitsCount > 10 ) )
        {
            throw new ArgumentException("This quantum computer can handle only ten QuBits", nameof( quBitsCount ));
        }

        this.QuBitsCount = quBitsCount;
        this.steps = [];
    }

    public int QuBitsCount { get; private set; }

    public void Initialize(QuState quState = QuState.Zero)
    {
    }

    public void Initialize(IEnumerable<QuState> quStates)
    {
    }

    public bool AddStep(QuStep step)
    {
        // TODO 
        this.steps.Add(step);
        return true;
    }

    public bool RemoveStep(QuStep step)
    {
        // TODO 
        this.steps.Remove(step);
        return true;
    }
}
