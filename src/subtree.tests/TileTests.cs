using NUnit.Framework;

namespace subtree.tests;

public class TileTests
{
    [Test]
    public void TileFirstTest()
    {
        var t = new Tile(0, 0, 0, true);
        Assert.IsTrue(t.GetChildren().Count == 4);
    }

    [Test]
    public void HasChildTest()
    {
        Assert.IsTrue(new Tile(0, 0, 0).HasChild(new Tile(1, 0, 0)));
        Assert.IsTrue(new Tile(1, 0, 0).HasChild(new Tile(2, 0, 0)));
        Assert.IsFalse(new Tile(1, 0, 0).HasChild(new Tile(2, 3, 0)));
        Assert.IsFalse(new Tile(1, 0, 0).HasChild(new Tile(2, 0, 3)));
        Assert.IsTrue(new Tile(1, 0, 0).HasChild(new Tile(3, 0, 0)));
        Assert.IsTrue(new Tile(1, 1, 1).HasChild(new Tile(2, 3, 3)));
        Assert.IsFalse(new Tile(1, 1, 1).HasChild(new Tile(2, 0, 0)));
    }

}
