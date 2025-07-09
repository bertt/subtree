using Newtonsoft.Json;
using NUnit.Framework;

namespace subtree.tests;

public class SubtreeReaderTests
{
    [Test]
    public void ReadOctreeSubtree0_0_0_0()
    {
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitOctree/0.0.0.0.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);
        var json = subtree.SubtreeJson;
        var subtreeJsonObject = JsonConvert.DeserializeObject<SubtreeJson>(subtree.SubtreeJson);

        // get the count of true in bitarray subtree.TileAvailability
        Assert.That(subtreeJsonObject.tileAvailability.availableCount == subtree.TileAvailability.Cast<bool>().Count(x => x == true));         var actual = subtree.TileAvailability.Count();

        // 80 bits
        var tileAvailability = subtree.TileAvailability.AsString();
        Assert.That(tileAvailability.Length == 80);
        Assert.That(tileAvailability == "11111000100000000100000011000000110000001000000000000000000000000100000010000000");

        var l = LevelOffset.GetNumberOfLevels(tileAvailability,ImplicitSubdivisionScheme.Octree);
        Assert.That(l == 3);

        // 80 bits
        var contentAvailability = subtree.ContentAvailability.AsString();
        Assert.That(contentAvailability.Length == 80);
        Assert.That(subtreeJsonObject.contentAvailability.First().availableCount == subtree.ContentAvailability.Cast<bool>().Count(x => x == true)); 
        Assert.That(contentAvailability == "01000000000000000100000010000000000000000000000000000000000000000000000000000000");

        var childSubtreeAvailability = subtree.ChildSubtreeAvailability.AsString();
        Assert.That(childSubtreeAvailability.Length == 512);
        Assert.That(childSubtreeAvailability == "00000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000100000010000000000000000000000000000000000000000000000001000000110000001000000000000000000000000000000000000000000000000100000010000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000000001000000100000000000000000000000000000000000000000000000010000001");
        // 512 is equal to 8*8*8
    
    
    }

    [Test]
    public void ReadSubtreeTestLevel3_5_0()
    {
        // see https://github.com/CesiumGS/3d-tiles-samples/blob/main/1.1/SparseImplicitQuadtree/screenshot/subtreeInfo.md
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitQuadtree/3.5.0.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);

        var l = LevelOffset.GetNumberOfLevels(subtree.TileAvailability.ToString(), ImplicitSubdivisionScheme.Quadtree);
        Assert.That(l == 3);

        // tile availability
        var t_0 = subtree.TileAvailability.AsString();
        Assert.That(t_0 == "110010110000000000110000");

        // content availability
        if (subtree.ContentAvailability != null){
            var c_0 = subtree.ContentAvailability.AsString();
            Assert.That(c_0 == "000000110000000000110000");
        }

        // Child Subtree Availability:
        Assert.That(subtree.ChildSubtreeAvailability == null);
    }

    [Test]
    public void ReadSubtreeTestLevel0_0_0()
    {
        // see https://github.com/CesiumGS/3d-tiles-samples/blob/main/1.1/SparseImplicitQuadtree/screenshot/subtreeInfo.md
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitQuadtree/0.0.0.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);
        Assert.That(subtree.SubtreeHeader.Magic == "subt");
        Assert.That(subtree.SubtreeHeader.Version == 1);
        Assert.That(subtree.SubtreeHeader.JsonByteLength== 312);
        Assert.That(subtree.SubtreeHeader.BinaryByteLength== 16);
        Assert.That(subtree.SubtreeBinary.Length == 16);

        // tile availability
        var t_0 = subtree.TileAvailability.AsString();
        Assert.That(t_0.Length == 24);

        // content availability
        Assert.That(subtree.ContentAvailability == null);

        // Child Subtree Availability:
        if(subtree.ChildSubtreeAvailability != null)
        {
            var c_0 = subtree.ChildSubtreeAvailability.AsString();
            Assert.That(c_0 == "0000000000000000011000000000011001100000000001100000000000000000");

            var childSubtreeAvailability = BitArray2DCreator.GetBitArray2D(subtree.ChildSubtreeAvailability.AsString());
            var expectedSubtreeFiles = childSubtreeAvailability.GetAvailableFiles();
            Assert.That(expectedSubtreeFiles.Count() == 8);
        }
    }

}