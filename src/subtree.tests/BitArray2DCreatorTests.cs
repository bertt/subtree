using NUnit.Framework;

namespace subtree.tests;

public class BitArray2DCreatorTests
{
    [Test]
    public void TestAvailability2D()
    {
        // https://github.com/CesiumGS/3d-tiles/tree/draft-1.1/specification/ImplicitTiling
        // morton index for level 1 (2*2)
        var mortonIndex = "0111";
        var bitArray2D = BitArray2DCreator.GetBitArray2D(mortonIndex);
        Assert.That(!bitArray2D.Get(0, 0));
        Assert.That(bitArray2D.Get(1, 0));
        Assert.That(bitArray2D.Get(0, 1));
        Assert.That(bitArray2D.Get(1, 1));
    }

    [Test]
    public void TestAvailability3D()
    {
        // https://github.com/CesiumGS/3d-tiles/tree/draft-1.1/specification/ImplicitTiling
        // morton index for level 1 (2*2*2)
        var mortonIndex = "10000000";
        var bitArray3D = BitArray3DCreator.GetBitArray3D(mortonIndex);
        Assert.That(bitArray3D.Get(0, 0, 0));
        Assert.That(!bitArray3D.Get(1, 0, 0));
        Assert.That(!bitArray3D.Get(0, 1, 1));
    }
}
