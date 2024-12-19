namespace Lyt.Quantics.Engine.Machine; 

public sealed class SubStage
{
    public Matrix<Complex> SubStageMatrix { get; private set; } = Matrix<Complex>.Build.Dense(1, 1);
}
