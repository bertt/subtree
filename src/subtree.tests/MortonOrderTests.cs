using NUnit.Framework;

namespace subtree.tests;


public class MortonOrderTests
{
    [TestCase(0, 0, 0, 0)]
    [TestCase(1, 0, 0, 1)]
    [TestCase(0, 1, 0, 2)]
    [TestCase(1, 1, 0, 3)]
    [TestCase(0, 0, 1, 4)]
    [TestCase(1, 0, 1, 5)]
    [TestCase(0, 1, 1, 6)]
    [TestCase(1, 1, 1, 7)]
    public void MortonOctreeTest(int x,  int y, int z, int expected)
    {
        var mortonOrderEncode = MortonOrder.Encode3D((ulong)x, (ulong)y, (ulong)z);
        Assert.That(mortonOrderEncode == expected);
    }

    [Test]
    public void MortonOctreeRoundTripTest()
    {
        var mortonIndex = (uint)0b010011010;
        var res = MortonOrder.Decode3D(mortonIndex);
        Assert.That(res.x == 2 && res.y == 7 && res.z == 0);
        Assert.That(MortonOrder.Encode3D(res.x, res.y,res.z) == mortonIndex);
    }

    // sample from:
    [Test]
    public void MortonRoundTripTest()
    {
        // sample from: https://github.com/CesiumGS/3d-tiles/blob/draft-1.1/specification/ImplicitTiling/AVAILABILITY.adoc#implicittiling-availability-indexing
        var mortonIndex = (uint)0b010011;
        var res = MortonOrder.Decode2D(mortonIndex);
        Assert.That(res.x == 5 && res.y == 1);
        Assert.That(MortonOrder.Encode2D(res.x, res.y) == mortonIndex);
    }

    [Test]
    public void MortonEncode2DTests()
    {
        Assert.That(MortonOrder.Encode2D(0, 0) == 0); // binary: 000
        Assert.That(MortonOrder.Encode2D(1, 0) == 1); // binary: 001
        Assert.That(MortonOrder.Encode2D(0, 1) == 2); // binary: 010
        Assert.That(MortonOrder.Encode2D(1, 1) == 3); // binary: 011
        Assert.That(MortonOrder.Encode2D(2, 0) == 4); // binary: 100
        Assert.That(MortonOrder.Encode2D(3, 0) == 5); // binary: 101
        Assert.That(MortonOrder.Encode2D(2, 1) == 6); // binary: 110
        Assert.That(MortonOrder.Encode2D(3, 1) == 7); // binary: 111
        Assert.That(MortonOrder.Encode2D(7, 5) == 55); // binary: 110111
        Assert.That(MortonOrder.Encode2D(65535, 65535) == 4294967295); // binary: 11111111111111111111111111111111
        Assert.That(MortonOrder.Encode2D(65535, 0) == 1431655765);  //// binary:  1010101010101010101010101010101
        Assert.That(MortonOrder.Encode2D(0, 65535) == 2863311530);  //// binary:  10101010101010101010101010101010

        Assert.That(MortonOrder.Encode2D(0, 65535) == 2863311530);  //// binary:  10101010101010101010101010101010

    }

    [Test]
    public void MortonEncode3DTests()
    {
        Assert.That(MortonOrder.Encode3D(0, 0, 0) == 0); // binary: 000
        Assert.That(MortonOrder.Encode3D(1, 0, 0) == 1); // binary: 010
        Assert.That(MortonOrder.Encode3D(0, 1, 0) == 2); // binary: 010
        Assert.That(MortonOrder.Encode3D(1, 1, 0) == 3); // binary: 010
        Assert.That(MortonOrder.Encode3D(0, 0, 1) == 4); // binary: 010
        Assert.That(MortonOrder.Encode3D(1, 0, 1) == 5); // binary: 010
        Assert.That(MortonOrder.Encode3D(0, 1, 1) == 6); // binary: 010
        Assert.That(MortonOrder.Encode3D(1, 1, 1) == 7); // binary: 010

    }

    [Test]
    public void MortonDecode2DTests()
    {
        Assert.That(MortonOrder.Decode2D(0) == (0, 0)); // binary: 000
        Assert.That(MortonOrder.Decode2D(1) == (1, 0)); // binary: 001
        Assert.That(MortonOrder.Decode2D(2) == (0, 1)); // binary: 010
        Assert.That(MortonOrder.Decode2D(3) == (1, 1)); // binary: 011
        Assert.That(MortonOrder.Decode2D(4) == (2, 0)); // binary: 100
        Assert.That(MortonOrder.Decode2D(5) == (3, 0)); // binary: 101
        Assert.That(MortonOrder.Decode2D(6) == (2, 1)); // binary: 110
        Assert.That(MortonOrder.Decode2D(7) == (3, 1)); // binary: 111
        Assert.That(MortonOrder.Decode2D(55) == (7, 5)); // binary: 110111
        Assert.That(MortonOrder.Decode2D(4294967295) == (65535, 65535)); // binary: 11111111111111111111111111111111
        Assert.That(MortonOrder.Decode2D(1431655765) == (65535, 0)); // binary:  1010101010101010101010101010101
        Assert.That(MortonOrder.Decode2D(2863311530) == (0, 65535)); // binary: 10101010101010101010101010101010
    }
}
