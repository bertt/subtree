using NUnit.Framework;

namespace subtree.tests;

public class BitArray2DCreatorTests
{
    [Test]
    public void TestAvailability()
    {
        // https://github.com/CesiumGS/3d-tiles/tree/draft-1.1/specification/ImplicitTiling
        var mortonIndex = "0111";
        var bitArray2D = BitArray2DCreator.GetBitArray2D(mortonIndex);
        Assert.That(!bitArray2D.Get(0, 0));
        Assert.That(bitArray2D.Get(1, 0));
        Assert.That(bitArray2D.Get(0, 1));
        Assert.That(bitArray2D.Get(1, 1));
    }
}
