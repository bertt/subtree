using NUnit.Framework;

namespace subtree.tests;

public class AvailabilityLevelTests
{
    [Test]
    public void TileAvailabilityToContentAvailabilityTest2()
    {
        // arrange
        var level0 = new AvailabilityLevel(0);
        var level1 = new AvailabilityLevel(1);
        level1.BitArray2D.Set(1, 0, true);
        level1.BitArray2D.Set(1, 1, true);
        var level2 = new AvailabilityLevel(2);
        level2.BitArray2D.Set(0, 0, true);
        level2.BitArray2D.Set(0, 3, true);

        var contentAvailabilitylevels = new AvailabilityLevels() { level0, level1, level2 };
        var mortonContent = contentAvailabilitylevels.ToMortonIndex();
        Assert.IsTrue(mortonContent == "001011000000000100000");
        // act
        AvailabilityLevels tileAvailabilitylevels = ContentToTileAvailability.GetTileAvailabilityLevels(contentAvailabilitylevels);

        // assert
        var mortenTileAvailability = tileAvailabilitylevels.ToMortonIndex();
        Assert.IsTrue(mortenTileAvailability == "111111000000000100000");
    }




    [Test]
    public void TileAvailabilityToContentAvailabilityTest1()
    {
        // arrange
        var level0 = new AvailabilityLevel(0);
        level0.BitArray2D.Set(0, 0, true);

        var contentAvailabilitylevels = new AvailabilityLevels() { level0};

        // act
        AvailabilityLevels tileAvailabilitylevels = ContentToTileAvailability.GetTileAvailabilityLevels(contentAvailabilitylevels);

        // assert
        var tileAvailabilityLevel0 = tileAvailabilitylevels.Where(z => z.Level == 0).FirstOrDefault();
        Assert.IsTrue(tileAvailabilityLevel0.BitArray2D.IsAvailable());
        var res = tileAvailabilitylevels.ToMortonIndex();
        Assert.IsTrue(res == "1");
    }

    [Test]
    public void TileAvailabilityToContentAvailabilityTest()
    {
        // arrange
        var level0 = new AvailabilityLevel(0);
        var level1 = new AvailabilityLevel(1);
        level1.BitArray2D.Set(0, 0, true);
        var p = level1.ToMortonIndex();

        var contentAvailabilitylevels = new AvailabilityLevels() { level0, level1 };

        // act
        var tileAvailabilitylevels = ContentToTileAvailability.GetTileAvailabilityLevels(contentAvailabilitylevels);

        // assert
        var tileAvailabilityLevel0 = tileAvailabilitylevels.Where(z => z.Level == 0).FirstOrDefault();
        Assert.IsTrue(tileAvailabilityLevel0.BitArray2D.IsAvailable());
        var res = tileAvailabilitylevels.ToMortonIndex();
        Assert.IsTrue(res == "11000");
    }


    [Test]
    public void AvailabilityLevelToMortonTest()
    {
        var availabilityLevels = new AvailabilityLevels();

        var availabilityLevel0 = new AvailabilityLevel(0);
        availabilityLevels.Add(availabilityLevel0);

        var availabilityLevel1 = new AvailabilityLevel(1);
        availabilityLevel1.BitArray2D.Set(0, 0, true);
        availabilityLevels.Add(availabilityLevel1);

        Assert.IsTrue(availabilityLevel1.ToMortonIndex() == "1000");
        Assert.IsTrue(availabilityLevels.ToMortonIndex() == "01000");
    }
}
