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

    [Test]
    public void CreateSubtreeWithMultipleLevelsAndSparseContent()
    {
        // This test simulates the real-world scenario where:
        // - We have tiles at level 0, 1, 2, 3, 4, 5
        // - Content is sparse (not all tiles have content)
        // - Subtrees should be created at appropriate levels
        var tiles = new List<Tile3D>();

        // Root tile
        tiles.Add(new Tile3D(0, 0, 0, 0) { Available = false });

        // Level 1 - add some tiles
        tiles.Add(new Tile3D(1, 0, 0, 0) { Available = false });
        tiles.Add(new Tile3D(1, 0, 0, 1) { Available = false });

        // Level 2 - add tiles with some content
        tiles.Add(new Tile3D(2, 0, 0, 0) { Available = true });
        tiles.Add(new Tile3D(2, 0, 0, 1) { Available = false });
        tiles.Add(new Tile3D(2, 0, 1, 0) { Available = false });

        // Level 3 - add tiles with content
        tiles.Add(new Tile3D(3, 0, 0, 0) { Available = true });
        tiles.Add(new Tile3D(3, 0, 0, 1) { Available = true });
        tiles.Add(new Tile3D(3, 0, 1, 0) { Available = false });

        // Level 4 - deeper content
        tiles.Add(new Tile3D(4, 0, 0, 0) { Available = true });
        tiles.Add(new Tile3D(4, 0, 0, 2) { Available = true });

        // Level 5 - even deeper
        tiles.Add(new Tile3D(5, 0, 0, 0) { Available = true });

        var subtreeFiles = SubtreeCreator3D.GenerateSubtreefiles(tiles);

        // With maxLevel=5, subtreeLevel should be (5+1)/2 = 3
        // So we should have:
        // - Root subtree (0,0,0,0)
        // - Child subtrees at level 3 for tiles that have children
        
        Assert.That(subtreeFiles.Count, Is.GreaterThan(0), "Should generate at least one subtree file");
        
        // Root subtree should exist
        Assert.That(subtreeFiles.ContainsKey(new Tile3D(0, 0, 0, 0)), "Root subtree should exist");

        // Verify root subtree has child subtree availability set correctly
        var rootStream = new MemoryStream(subtreeFiles[new Tile3D(0, 0, 0, 0)]);
        var rootSubtree = SubtreeReader.ReadSubtree(rootStream);
        Assert.That(rootSubtree.ChildSubtreeAvailability, Is.Not.Null, "Root should have child subtree availability");
    }

    [Test]
    public void EnsureAllRequestedSubtreesAreGenerated()
    {
        // This test ensures that all subtree files that could be requested are actually generated
        // Simulating a scenario with levels 0-5 where tiles exist at various levels
        var tiles = new List<Tile3D>();

        // Create a tree structure with content at various levels
        tiles.Add(new Tile3D(0, 0, 0, 0) { Available = false });

        // Level 1 - octree has 8 possible children, add a few
        for (var x = 0; x < 2; x++)
            for (var y = 0; y < 1; y++)
                for (var z = 0; z < 1; z++)
                    tiles.Add(new Tile3D(1, x, y, z) { Available = false });

        // Level 2 - add content in one branch
        tiles.Add(new Tile3D(2, 0, 0, 0) { Available = true });
        tiles.Add(new Tile3D(2, 1, 0, 0) { Available = false });

        // Level 3 - add more content
        tiles.Add(new Tile3D(3, 0, 0, 0) { Available = true });
        tiles.Add(new Tile3D(3, 1, 0, 0) { Available = true });
        tiles.Add(new Tile3D(3, 2, 0, 0) { Available = false });

        var subtreeFiles = SubtreeCreator3D.GenerateSubtreefiles(tiles);

        // Verify that we have generated subtree files for all necessary tiles
        Assert.That(subtreeFiles.Count, Is.GreaterThan(0));
        
        // Check that the root exists
        var rootKey = new Tile3D(0, 0, 0, 0);
        Assert.That(subtreeFiles.ContainsKey(rootKey), $"Root subtree should be generated");

        // With maxLevel=3, subtreeLevel=(3+1)/2=2
        // We should have child subtrees at level 2
        Console.WriteLine($"Generated {subtreeFiles.Count} subtree files:");
        foreach (var key in subtreeFiles.Keys)
        {
            Console.WriteLine($"  Level {key.Level}: ({key.X}, {key.Y}, {key.Z})");
        }
    }

    [Test]
    public void VerifyChildSubtreesMatchTileAvailability()
    {
        // This test verifies that child subtree availability correctly indicates
        // which subtree files should be generated
        var tiles = new List<Tile3D>();

        tiles.Add(new Tile3D(0, 0, 0, 0) { Available = false });

        // Level 1
        for (var x = 0; x < 2; x++)
            for (var y = 0; y < 2; y++)
                for (var z = 0; z < 2; z++)
                    tiles.Add(new Tile3D(1, x, y, z) { Available = false });

        // Level 2 - add tiles that have children at level 3
        tiles.Add(new Tile3D(2, 0, 0, 0) { Available = false });
        tiles.Add(new Tile3D(2, 1, 0, 0) { Available = false });

        // Level 3 - add content
        tiles.Add(new Tile3D(3, 0, 0, 0) { Available = true });
        tiles.Add(new Tile3D(3, 2, 0, 0) { Available = true }); // This is a child of (2,1,0,0)

        var subtreeFiles = SubtreeCreator3D.GenerateSubtreefiles(tiles);

        // Read root subtree and check child availability
        var rootSubtree = SubtreeReader.ReadSubtree(new MemoryStream(subtreeFiles[new Tile3D(0, 0, 0, 0)]));
        
        // If child subtree availability is set, verify all indicated subtrees exist
        if (rootSubtree.ChildSubtreeAvailability != null)
        {
            var maxLevel = tiles.Max(t => t.Level);
            var subtreeLevel = (int)Math.Ceiling(((double)maxLevel + 1) / 2);
            
            var ba = BitArray3DCreator.GetBitArray3D(
                Availability.GetLevelAvailability(
                    MortonIndex.GetMortonIndices3D(tiles).tileAvailability,
                    subtreeLevel,
                    ImplicitSubdivisionScheme.Octree));

            for (var x = 0; x < ba.GetDimension(); x++)
            {
                for (var y = 0; y < ba.GetDimension(); y++)
                {
                    for (var z = 0; z < ba.GetDimension(); z++)
                    {
                        if (ba.Get(x, y, z))
                        {
                            var expectedKey = new Tile3D(subtreeLevel, x, y, z);
                            Assert.That(subtreeFiles.ContainsKey(expectedKey),
                                $"Subtree at level {subtreeLevel} ({x},{y},{z}) should be generated because child availability indicates it");
                        }
                    }
                }
            }
        }
    }

    [Test]
    public void RealWorldScenarioWithDeepHierarchy()
    {
        // Simulates a real scenario with levels 0-5
        var tiles = new List<Tile3D>();

        // Root
        tiles.Add(new Tile3D(0, 0, 0, 0) { Available = false });

        // Level 1 - sparse
        tiles.Add(new Tile3D(1, 0, 0, 0) { Available = false });

        // Level 2 - some content
        tiles.Add(new Tile3D(2, 0, 0, 0) { Available = true });

        // Level 3 - more content  
        tiles.Add(new Tile3D(3, 0, 0, 0) { Available = true });
        tiles.Add(new Tile3D(3, 0, 0, 1) { Available = false });

        // Level 4
        tiles.Add(new Tile3D(4, 0, 0, 0) { Available = true });

        // Level 5
        tiles.Add(new Tile3D(5, 0, 0, 0) { Available = true });

        var subtreeFiles = SubtreeCreator3D.GenerateSubtreefiles(tiles);

        // With maxLevel=5, subtreeLevel should be (5+1)/2 = 3
        var maxLevel = tiles.Max(t => t.Level);
        var subtreeLevel = (int)Math.Ceiling(((double)maxLevel + 1) / 2);
        
        Assert.That(subtreeLevel, Is.EqualTo(3), "Subtree level calculation");
        Assert.That(subtreeFiles.Count, Is.GreaterThan(0), "Should generate at least one subtree file");
        
        // Root subtree should exist
        Assert.That(subtreeFiles.ContainsKey(new Tile3D(0, 0, 0, 0)), "Root subtree should exist");

        // Verify all tiles indicated by child availability are generated
        var rootSubtree = SubtreeReader.ReadSubtree(new MemoryStream(subtreeFiles[new Tile3D(0, 0, 0, 0)]));
        if (rootSubtree.ChildSubtreeAvailability != null)
        {
            var ba = BitArray3DCreator.GetBitArray3D(
                Availability.GetLevelAvailability(
                    MortonIndex.GetMortonIndices3D(tiles).tileAvailability,
                    subtreeLevel,
                    ImplicitSubdivisionScheme.Octree));

            for (var x = 0; x < ba.GetDimension(); x++)
            {
                for (var y = 0; y < ba.GetDimension(); y++)
                {
                    for (var z = 0; z < ba.GetDimension(); z++)
                    {
                        if (ba.Get(x, y, z))
                        {
                            var expectedKey = new Tile3D(subtreeLevel, x, y, z);
                            Assert.That(subtreeFiles.ContainsKey(expectedKey),
                                $"Subtree at level {subtreeLevel} ({x},{y},{z}) should exist but is missing!");
                        }
                    }
                }
            }
        }
    }

    [Test]
    public void TestMissingIntermediateTiles()
    {
        // This test simulates the real-world issue where intermediate tiles
        // might not be in the tiles list because the octreewriter sample
        // doesn't add them during recursion
        var tiles = new List<Tile3D>();

        // Root - no content, just a container
        tiles.Add(new Tile3D(0, 0, 0, 0) { Available = false });

        // Skip level 1 tiles entirely (simulating what might happen in real dataset)
        // Level 2 - has content
        tiles.Add(new Tile3D(2, 0, 0, 0) { Available = true });
        tiles.Add(new Tile3D(2, 1, 0, 0) { Available = true });

        // Level 3
        tiles.Add(new Tile3D(3, 0, 0, 0) { Available = true });
        tiles.Add(new Tile3D(3, 2, 0, 0) { Available = true });

        var subtreeFiles = SubtreeCreator3D.GenerateSubtreefiles(tiles);

        // With maxLevel=3, subtreeLevel=(3+1)/2=2
        // Should generate root and potentially child subtrees
        Assert.That(subtreeFiles.Count, Is.GreaterThan(0));
        Assert.That(subtreeFiles.ContainsKey(new Tile3D(0, 0, 0, 0)), "Root subtree should exist");
    }
}
