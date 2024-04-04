using NUnit.Framework;

namespace subtree.tests;

public class TileTests
{
    [Test]
    public void GetParentTest00()
    {
        var from = new Tile(6, 24, 37);
        var to = new Tile(9, 197, 296);
        Assert.That(from.HasChild(to));
    }

    [Test]
    public void TileFirstTest()
    {
        var t = new Tile(0, 0, 0, true);
        Assert.That(t.GetChildren().Count == 4);
    }

    [Test]
    public void HasChildTest()
    {
        Assert.That(new Tile(0, 0, 0).HasChild(new Tile(1, 0, 0)));
        Assert.That(new Tile(1, 0, 0).HasChild(new Tile(2, 0, 0)));
        Assert.That(!new Tile(1, 0, 0).HasChild(new Tile(2, 3, 0)));
        Assert.That(!new Tile(1, 0, 0).HasChild(new Tile(2, 0, 3)));
        Assert.That(new Tile(1, 0, 0).HasChild(new Tile(3, 0, 0)));
        Assert.That(new Tile(1, 1, 1).HasChild(new Tile(2, 3, 3)));
        Assert.That(!new Tile(1, 1, 1).HasChild(new Tile(2, 0, 0)));
    }

    [Test]
    public void GetParentTest0()
    {
        var from = new Tile(6, 24, 37);
        var to = new Tile(9, 197, 296);
        var rel = SubtreeCreator.GetRelativeTile(from, to);
        Assert.That(rel.Z == 3 && rel.X == 5 && rel.Y == 0);
    }

    [Test]
    public void GetParentTest()
    {
        var from = new Tile(1, 1, 1);
        var to = new Tile(3, 7, 7);

        var rel = SubtreeCreator.GetRelativeTile(from, to);
        Assert.That(rel.Z == 2 && rel.X == 3 && rel.Y == 3);
    }

    [Test]
    public void GetRelativeTileTest2()
    {
        var from = new Tile(1, 0, 0);
        var to = new Tile(2, 0, 1);
        var rel = SubtreeCreator.GetRelativeTile(from, to);

        Assert.That(rel.Z == 1 && rel.X == 0 && rel.Y == 1);
    }

    [Test]
    public void GetRelativeTest3()
    {
        var from = new Tile(1, 0, 1);
        var to = new Tile(2, 0, 2);
        var rel = SubtreeCreator.GetRelativeTile(from, to);
        Assert.That(rel.Z == 1 && rel.X == 0 && rel.Y == 0);
    }

    [Test]
    public void GetParentTest4()
    {
        var from = new Tile(1, 0, 1);
        var to = new Tile(2, 0, 3);
        var rel = SubtreeCreator.GetRelativeTile(from, to);
        Assert.That(rel.Z == 1 && rel.X == 0 && rel.Y == 1);
    }
}