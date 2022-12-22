using NUnit.Framework;

namespace subtree.tests;

public class BitArrayTests
{
    [Test]
    public void TestBitArrayFromString()
    {
        var ba = BitArrayCreator.FromString("10110000");
        Assert.IsTrue(ba.Length == 8);
        Assert.IsTrue(ba[0]);
        Assert.IsFalse(ba[1]);
    }
}
