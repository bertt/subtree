using NUnit.Framework;

namespace subtree.tests;

public class MortonIndexTests
{
    [Test]
    public void MortonIndexTest()
    {
        // arrange
        var t = new Tile(0, 0, 0);
        t.Available = false;

        var tiles = new List<Tile>() { t };

        for (var x = 0; x < 2; x++)
            for (var y = 0; y < 2; y++)
            {
                var t2 = new Tile(1, x, y);
                t2.Available = true;
                tiles.Add(t2);
            }


        // act
        var mortonIndices = MortonIndex.GetMortonIndices(tiles);
        var mortonIndicesBytes = MortonIndex.GetMortonIndexAsBytes(tiles);

        // assert
        Assert.That(mortonIndices.tileAvailability == "11111");
        Assert.That(mortonIndices.contentAvailability == "01111");
        Assert.That(mortonIndicesBytes.tileAvailability.Length == 1);
    }

    [Test]
    public void MortonIndex3DTest()
    {
        var tile3d = new Tile3D(0, 0, 0, 0);
        tile3d.Available = false;

        var tiles = new List<Tile3D>() { tile3d };


        for (var x = 0; x < 2; x++)
            for (var y = 0; y < 2; y++)
                for (var z = 0; z < 2; z++)
                {
                    var t = new Tile3D(1, x, y, z);
                    t.Available = true;
                    tiles.Add(t);
                }

        var mortonIndices = MortonIndex.GetMortonIndices3D(tiles);

        // act
        var mortonIndicesBytes = MortonIndex.GetMortonIndexAsBytes3D(tiles);        
        Assert.That(mortonIndices.tileAvailability == "111111111");
        Assert.That(mortonIndices.contentAvailability == "011111111");
        Assert.That(mortonIndicesBytes.tileAvailability.Length == 2);
    }

    [Test]
    public void MortonIndex3DTest1()
    {
        var tile3d = new Tile3D(0, 0, 0, 0);
        tile3d.Available = false;

        var tiles = new List<Tile3D>() { tile3d };
        var t = new Tile3D(1, 0, 1, 0);
        t.Available = true;
        tiles.Add(t);

        var mortonIndices = MortonIndex.GetMortonIndices3D(tiles);

        // act
        var mortonIndicesBytes = MortonIndex.GetMortonIndexAsBytes3D(tiles);
        Assert.That(mortonIndices.tileAvailability == "100100000");
        Assert.That(mortonIndices.contentAvailability == "000100000");
        Assert.That(mortonIndicesBytes.tileAvailability.Length == 2);
    }


}
