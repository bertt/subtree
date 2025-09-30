using NUnit.Framework;

namespace subtree.tests;
public class SubtreeCreatorTests
{

    [Test]
    public void CreateSubtreeTest2()
    {
        var tile = new Tile(0, 0, 0);
        tile.Available = false;
        var tiles = new List<Tile> { tile };

        for(var x= 0; x < 2; x++)
            for(var y= 0; y < 2; y++)
            {
                var t2 = new Tile(1, x, y);
                t2.Available = true;
                tiles.Add(t2);
            }

        var subtreeFiles = SubtreeCreator.GenerateSubtreefiles(tiles);
        Assert.That(subtreeFiles, Is.Not.EqualTo(null));
        var stream = new MemoryStream(subtreeFiles.FirstOrDefault().Value);
        var subtree = SubtreeReader.ReadSubtree(stream);
        Assert.That(subtree.TileAvailability[0]);
        Assert.That(!subtree.ContentAvailability[0]);
        Assert.That(subtree.ChildSubtreeAvailability == null);
    }


    [Test]
    public void CreateSubtreeTest()
    {
        var tile = new Tile(0, 0, 0);
        tile.Available = true;
        var subtreeFiles = SubtreeCreator.GenerateSubtreefiles(new List<Tile> { tile });
        Assert.That(subtreeFiles, Is.Not.EqualTo(null));
        var stream = new MemoryStream(subtreeFiles.FirstOrDefault().Value);
        var subtree = SubtreeReader.ReadSubtree(stream);
        Assert.That(subtree.TileAvailability[0]);
        Assert.That(subtree.ContentAvailability[0]);
        Assert.That(subtree.ChildSubtreeAvailability == null);
    }

    [Test]
    public void CreateSubtreeRootTest()
    {
        var rootTile = new Tile(0, 0, 0);
        var tiles = new List<Tile> { rootTile };

        // level 1
        tiles.Add(new Tile(1, 0, 0));
        tiles.Add(new Tile(1, 0, 1));
        tiles.Add(new Tile(1, 1, 0));
        tiles.Add(new Tile(1, 1, 1));

        // level 2
        var l2 = new Tile(2, 0, 0);
        l2.Available = true;
        tiles.Add(l2);

        // level 3
        var l3 = new Tile(3, 0, 0);
        l3.Available = true;
        tiles.Add(l3);

        // asct
        var subtreeFiles = SubtreeCreator.GenerateSubtreefiles(tiles);

        // read root subtree file
        Assert.That(subtreeFiles.Count == 2);
        var stream = new MemoryStream(subtreeFiles.FirstOrDefault().Value);
        var subtree = SubtreeReader.ReadSubtree(stream);

        Assert.That(subtree.TileAvailability[0]);
        Assert.That(subtree.TileAvailability[1]);
        Assert.That(!subtree.TileAvailability[2]);
        Assert.That(!subtree.TileAvailability[3]);
        Assert.That(!subtree.TileAvailability[4]);

        Assert.That(subtree.ContentAvailabilityConstant == 0);
        Assert.That(subtree.ChildSubtreeAvailability != null);
        Assert.That(subtree.ChildSubtreeAvailability[0]);
        Assert.That(!subtree.ChildSubtreeAvailability[1]);
        Assert.That(!subtree.ChildSubtreeAvailability[2]);
        Assert.That(!subtree.ChildSubtreeAvailability[3]);

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
        var rootTile = new Tile(0, 0, 0);
        var tiles = new List<Tile> { rootTile };

        // level 1
        tiles.Add(new Tile(1, 0, 0));
        tiles.Add(new Tile(1, 0, 1));
        tiles.Add(new Tile(1, 1, 0));
        tiles.Add(new Tile(1, 1, 1));

        // level 2
        var l2 = new Tile(2, 0, 0);
        l2.Available = true;
        tiles.Add(l2);

        var subtreeTiles = SubtreeCreator.GetSubtreeTiles(tiles, new Tile(1, 0, 0));
        var subtreeTile = subtreeTiles.FirstOrDefault();
        Assert.That(subtreeTile.Z == 0);
    }
}
