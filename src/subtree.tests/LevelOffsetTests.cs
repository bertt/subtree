using NUnit.Framework;

namespace subtree.tests;

public class LevelOffsetTests
{
    [Test]
    public void LevelOffsetTest()
    {
        Assert.IsTrue(LevelOffset.GetLevelOffset(0) == 0);
        Assert.IsTrue(LevelOffset.GetLevelOffset(1) == 1);
        Assert.IsTrue(LevelOffset.GetLevelOffset(2) == 5);
        Assert.IsTrue(LevelOffset.GetLevelOffset(3) == 21);
        Assert.IsTrue(LevelOffset.GetLevelOffset(4) == 85);
    }

    [Test]
    public void LevelOffsetTestOctree()
    {
        Assert.IsTrue(LevelOffset.GetLevelOffset(0, ImplicitSubdivisionScheme.Octree) == 0);
        Assert.IsTrue(LevelOffset.GetLevelOffset(1, ImplicitSubdivisionScheme.Octree) == 1);
        Assert.IsTrue(LevelOffset.GetLevelOffset(2, ImplicitSubdivisionScheme.Octree) == 9);
        Assert.IsTrue(LevelOffset.GetLevelOffset(3, ImplicitSubdivisionScheme.Octree) == 73);
        Assert.IsTrue(LevelOffset.GetLevelOffset(4, ImplicitSubdivisionScheme.Octree) == 585);
    }

    [Test]
    public void TileAvailabilityTests()
    {
        var contentavailability = "101100000100110010000000";
        var l = LevelOffset.GetNumberOfLevels(contentavailability);
        Assert.IsTrue(l==3);

        contentavailability = "11000000";
        l = LevelOffset.GetNumberOfLevels(contentavailability);
        Assert.IsTrue(l == 2);

        contentavailability = "10000000";
        l = LevelOffset.GetNumberOfLevels(contentavailability);
        Assert.IsTrue(l == 2);
    }
}
