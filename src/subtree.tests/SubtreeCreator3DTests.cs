using NUnit.Framework;

namespace subtree.tests;
public class SubtreeCreator3DTests
{
    [Test]
    public void CreateSubtreeTest()
    {
        var tile = new Tile3D(0, 0, 0, 0);
        tile.Available = true;
        var subtreeFiles = SubtreeCreator3D.GenerateSubtreefiles(new List<Tile3D> { tile });
        Assert.That(subtreeFiles, Is.Not.EqualTo(null));
        var stream = new MemoryStream(subtreeFiles.FirstOrDefault().Value);
        var subtree = SubtreeReader.ReadSubtree(stream);
        Assert.That(subtree.TileAvailability[0]);
        Assert.That(subtree.ContentAvailability[0]);
        Assert.That(subtree.ChildSubtreeAvailability == null);
    }

    [Test]
    public void CreateSubtreeTest2()
    {
        var tile = new Tile3D(0, 0, 0, 0);
        tile.Available = false;
        var tiles = new List<Tile3D> { tile };

        for (var x = 0; x < 2; x++)
            for (var y = 0; y < 2; y++)
                for (var z = 0; z < 2; z++)
                {
                    var t2 = new Tile3D(1, x, y, z);
                    t2.Available = true;
                    tiles.Add(t2);
                }

        var subtreeFiles = SubtreeCreator3D.GenerateSubtreefiles(tiles);
        Assert.That(subtreeFiles, Is.Not.EqualTo(null));
        var stream = new MemoryStream(subtreeFiles.FirstOrDefault().Value);
        var subtree = SubtreeReader.ReadSubtree(stream);
        Assert.That(subtree.TileAvailability[0]);
        Assert.That(!subtree.ContentAvailability[0]);
        Assert.That(subtree.ChildSubtreeAvailability == null);
    }

    [Test]
    public void CreateSubtreeRootTest()
    {
        var rootTile = new Tile3D(0, 0, 0, 0);
        var tiles = new List<Tile3D> { rootTile };

        // level 1 - octree has 8 children
        for (var x = 0; x < 2; x++)
            for (var y = 0; y < 2; y++)
                for (var z = 0; z < 2; z++)
                {
                    tiles.Add(new Tile3D(1, x, y, z));
                }

        // level 2
        var l2 = new Tile3D(2, 0, 0, 0);
        l2.Available = true;
        tiles.Add(l2);

        // level 3
        var l3 = new Tile3D(3, 0, 0, 0);
        l3.Available = true;
        tiles.Add(l3);

        // act
        var subtreeFiles = SubtreeCreator3D.GenerateSubtreefiles(tiles);

        // read root subtree file
        Assert.That(subtreeFiles.Count == 2);
        var stream = new MemoryStream(subtreeFiles.FirstOrDefault().Value);
        var subtree = SubtreeReader.ReadSubtree(stream);

        Assert.That(subtree.TileAvailability[0]);
        Assert.That(subtree.TileAvailability[1]);
        Assert.That(!subtree.TileAvailability[2]);
        
        Assert.That(subtree.ContentAvailabilityConstant == 0);
        Assert.That(subtree.ChildSubtreeAvailability != null);
        Assert.That(subtree.ChildSubtreeAvailability[0]);
        Assert.That(!subtree.ChildSubtreeAvailability[1]);

        // read child subtree file
        var streamChild = new MemoryStream(subtreeFiles.LastOrDefault().Value);
        var subtreeChild = SubtreeReader.ReadSubtree(streamChild);
        Assert.That(subtreeChild.ContentAvailability[0]);
        Assert.That(subtreeChild.TileAvailability[0]);
        Assert.That(subtreeChild.ContentAvailability[1]);
        Assert.That(subtreeChild.TileAvailability[1]);
    }

    [Test]
    public void GetSubtreeTilesTest()
    {
        var rootTile = new Tile3D(0, 0, 0, 0);
        var tiles = new List<Tile3D> { rootTile };

        // level 1
        for (var x = 0; x < 2; x++)
            for (var y = 0; y < 2; y++)
                for (var z = 0; z < 2; z++)
                {
                    tiles.Add(new Tile3D(1, x, y, z));
                }

        // level 2
        var l2 = new Tile3D(2, 0, 0, 0);
        l2.Available = true;
        tiles.Add(l2);

        var subtreeTiles = SubtreeCreator3D.GetSubtreeTiles(tiles, new Tile3D(1, 0, 0, 0));
        var subtreeTile = subtreeTiles.FirstOrDefault();
        Assert.That(subtreeTile.Level == 0);
    }

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
