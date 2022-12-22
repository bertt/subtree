using NUnit.Framework;

namespace subtree.tests;

public class MortonIndexTests
{
    [Test]
    public void MortonIndexTest()
    {
        // arrange
        var t = new Tile(0, 0, 0);
        t.Available = true;

        // act
        var mortonIndices = MortonIndex.GetMortonIndices(new List<Tile> { t });
        var mortonIndicesBytes = MortonIndex.GetMortonIndexAsBytes(new List<Tile> { t });

        // assert
        Assert.IsTrue(mortonIndices.tileAvailability == "1");
        Assert.IsTrue(mortonIndices.contentAvailability == "1");
        Assert.IsTrue(mortonIndicesBytes.tileAvailability.Length == 1);
    }
}
