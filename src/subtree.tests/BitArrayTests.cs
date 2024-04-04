using NUnit.Framework;

namespace subtree.tests;

public class BitArrayTests
{
    [Test]
    public void TestBitArrayFromString()
    {
        var ba = BitArrayCreator.FromString("10110000");
        Assert.That(ba.Length == 8);
        Assert.That(ba[0]);
        Assert.That(!ba[1]);
    }
}
