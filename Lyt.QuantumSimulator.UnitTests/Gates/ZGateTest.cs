namespace Lyt.QuantumSimulator.UnitTests.Gates;

[TestClass()]
public class ZGateTest
{
    [TestMethod()]
    public void FalseInputTest()
    {
        var q = new Qubit(false);
        new ZGate().Apply(q);
        var p = q.Peek();
        if ( p is null )
        {
            Assert.Fail();
        }

        Assert.AreEqual(new Complex(1, 0), p.X);
        Assert.AreEqual(new Complex(0, 0), p.Y);
        Assert.IsFalse(q.Measure());
    }

    [TestMethod()]
    public void TrueInputTest()
    {
        var q = new Qubit(true);
        new ZGate().Apply(q);
        var p = q.Peek();
        if (p is null)
        {
            Assert.Fail();
        }

        Assert.AreEqual(new Complex(0, 0), p.X);
        Assert.AreEqual(new Complex(-1, 0), p.Y);
        Assert.IsTrue(q.Measure());
    }
}