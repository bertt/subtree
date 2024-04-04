using NUnit.Framework;

namespace subtree.tests;

public class LevelOffsetTests
{
    [Test]
    public void LevelOffsetTest()
    {
        Assert.That(LevelOffset.GetLevelOffset(0) == 0);
        Assert.That(LevelOffset.GetLevelOffset(1) == 1);
        Assert.That(LevelOffset.GetLevelOffset(2) == 5);
        Assert.That(LevelOffset.GetLevelOffset(3) == 21);
        Assert.That(LevelOffset.GetLevelOffset(4) == 85);
    }

    [Test]
    public void LevelOffsetTestOctree()
    {
        Assert.That(LevelOffset.GetLevelOffset(0, ImplicitSubdivisionScheme.Octree) == 0);
        Assert.That(LevelOffset.GetLevelOffset(1, ImplicitSubdivisionScheme.Octree) == 1);
        Assert.That(LevelOffset.GetLevelOffset(2, ImplicitSubdivisionScheme.Octree) == 9);
        Assert.That(LevelOffset.GetLevelOffset(3, ImplicitSubdivisionScheme.Octree) == 73);
        Assert.That(LevelOffset.GetLevelOffset(4, ImplicitSubdivisionScheme.Octree) == 585);
    }

    [Test]
    public void TileAvailabilityTests()
    {
        var contentavailability = "101100000100110010000000";
        var l = LevelOffset.GetNumberOfLevels(contentavailability);
        Assert.That(l==3);

        contentavailability = "11000000";
        l = LevelOffset.GetNumberOfLevels(contentavailability);
        Assert.That(l == 2);

        contentavailability = "10000000";
        l = LevelOffset.GetNumberOfLevels(contentavailability);
        Assert.That(l == 2);

    }
}
