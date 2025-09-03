using NUnit.Framework;

namespace subtree.tests;

public class LevelTests
{
    [Test]
    public void QuadtreeLevelTests()
    {
        Assert.That(0, Is.EqualTo(Level.GetLevel(1)));
        Assert.That(1, Is.EqualTo(Level.GetLevel(4)));
        Assert.That(2, Is.EqualTo(Level.GetLevel(16)));
        Assert.That(3, Is.EqualTo(Level.GetLevel(64)));
        Assert.That(4, Is.EqualTo(Level.GetLevel(256)));
        Assert.That(5, Is.EqualTo(Level.GetLevel(1024)));
    }

    [Test]
    public void OctreeLevelTests()
    {
        Assert.That(0, Is.EqualTo(Level.GetLevel(1, ImplicitSubdivisionScheme.Octree)));
        Assert.That(1, Is.EqualTo(Level.GetLevel(8, ImplicitSubdivisionScheme.Octree)));
        Assert.That(2, Is.EqualTo(Level.GetLevel(4*4*4, ImplicitSubdivisionScheme.Octree)));
        Assert.That(3, Is.EqualTo(Level.GetLevel(8*8*8, ImplicitSubdivisionScheme.Octree)));
        Assert.That(4, Is.EqualTo(Level.GetLevel(16*16*16, ImplicitSubdivisionScheme.Octree)));
        Assert.That(5, Is.EqualTo(Level.GetLevel(32*32*32, ImplicitSubdivisionScheme.Octree)));
    }

}
