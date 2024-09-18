namespace Lyt.QuantumSimulator.Algorithms; 

public class DeutschAlgorithm
{
    public static bool IsBalanced(BinaryGate gate)
    {
        var q1 = new Qubit(false);
        var q2 = new Qubit(true);
        new HGate().Apply(q1);
        new HGate().Apply(q2);
        gate.Apply(q2, q1);
        new HGate().Apply(q1);
        return q1.Measure();
    }
}
