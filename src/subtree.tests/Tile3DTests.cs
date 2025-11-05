using NUnit.Framework;

namespace subtree.tests;

public class Tile3DTests
{
    [Test]
    public void Tile3DHasChildTest()
    {
        var parent = new Tile3D(0, 0, 0, 0);
        var child = new Tile3D(1, 0, 0, 0);
        Assert.That(parent.HasChild(child));

        var child2 = new Tile3D(1, 1, 1, 1);
        Assert.That(parent.HasChild(child2));

        var notChild = new Tile3D(0, 1, 0, 0);
        Assert.That(!parent.HasChild(notChild));
    }

    [Test]
    public void Tile3DParentTest()
    {
        var child = new Tile3D(1, 0, 0, 0);
        var parent = child.Parent();
        Assert.That(parent.Level == 0);
        Assert.That(parent.X == 0);
        Assert.That(parent.Y == 0);
        Assert.That(parent.Z == 0);

        var child2 = new Tile3D(2, 3, 3, 3);
        var parent2 = child2.Parent();
        Assert.That(parent2.Level == 1);
        Assert.That(parent2.X == 1);
        Assert.That(parent2.Y == 1);
        Assert.That(parent2.Z == 1);
    }

    [Test]
    public void GetRelativeTileTest()
    {
        var from = new Tile3D(1, 0, 0, 0);
        var to = new Tile3D(2, 1, 1, 1);
        var relative = SubtreeCreator3D.GetRelativeTile(from, to);
        Assert.That(relative.Level == 1);
        Assert.That(relative.X == 1);
        Assert.That(relative.Y == 1);
        Assert.That(relative.Z == 1);
    }

    [Test]
    public void FillTest()
    {
        var availability = "101";
        var filled = SubtreeCreator3D.Fill(availability, 10);
        Assert.That(filled == "1010000000");
        Assert.That(filled.Length == 10);
    }

}
