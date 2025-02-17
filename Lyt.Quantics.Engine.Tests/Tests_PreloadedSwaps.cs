namespace Lyt.Quantics.Engine.Tests;

[TestClass]
public sealed class Tests_PreloadedSwaps
{
    [TestMethod]
    public void Test_Serialization()
    {
        var swaps = new NestedDictionary<int, int, int, List<Swap>>
        {
            { 2, 0, 1, [new Swap(0, 1), new Swap(1, 0)] },
            { 3, 0, 1, [new Swap(0, 1), new Swap(1, 0)] },
            { 3, 0, 2, [new Swap(0, 2), new Swap(2, 0)] },
            { 3, 1, 2, [new Swap(1, 2), new Swap(2, 1)] }
        };

        string swapStrings = SerializationUtilities.Serialize(swaps);
        Debug.WriteLine(swapStrings);
        var data = SerializationUtilities.Deserialize<NestedDictionary<int, int, int, List<Swap>>>(swapStrings);
        Debug.WriteLine(data.GetType());
    }
}