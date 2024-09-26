namespace Lyt.Quantics.Engine.Machine;

public sealed class QuStage
{
    public QuStage() { /* Required for deserialization */ }

    public List<QuStageOperator> Operators { get; set; } = [];

    public QuRegister StageRegister { get; private set; } = new QuRegister(1);

    public Matrix<Complex> StageMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1,1);

    public bool Validate(out string message)
    {
        message = string.Empty;

        foreach (QuStageOperator stageOperator in this.Operators)
        {
            if (!stageOperator.Validate(out message))
            {
                return false;
            }
        }

        return true;
    }

    public bool Build(QuComputer computer, out string message)
    {
        message = string.Empty;

        try
        {
            int length = computer.QuBitsCount;
            this.StageRegister = new QuRegister(length);
            int powTwo = MathUtilities.TwoPower(length);
            this.StageMatrix = Matrix<Complex>.Build.Sparse(powTwo, powTwo, Complex.Zero);
        }
        catch (Exception ex)
        {
            message = string.Concat("Exception thrown: " + ex.Message) ;
            return false;
        } 

        return true;
    }

}
