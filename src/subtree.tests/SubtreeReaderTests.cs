using Newtonsoft.Json;
using NUnit.Framework;

namespace subtree.tests;

public class SubtreeReaderTests
{
    // from https://github.com/CesiumGS/cesium/tree/main/Specs/Data/Cesium3DTiles/Metadata/ImplicitSubtreeMetadata/subtrees
    [Test]
    public void ReadImplicitSubtreeWithMetadataJson()
    {
        var json = File.ReadAllText(@"testfixtures/implicitsubtree/0.0.0.json");
        // var subtree = SubtreeReader.ReadSubtree(subtreefile);
        var subtreeJsonObject = JsonConvert.DeserializeObject<SubtreeJson>(json);

        Assert.IsTrue(subtreeJsonObject.subtreeMetadata != null);
    }

    [Test]
    public void ReadSubtreeTestLevel3_5_0()
    {
        // see https://github.com/CesiumGS/3d-tiles-samples/blob/main/1.1/SparseImplicitQuadtree/screenshot/subtreeInfo.md
        var subtreefile = File.OpenRead(@"testfixtures/3.5.0.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);

        // tile availability
        var t_0 = subtree.TileAvailability.AsString();
        Assert.IsTrue(t_0 == "110010110000000000110000");

        // content availability
        if (subtree.ContentAvailability != null){
            var c_0 = subtree.ContentAvailability.AsString();
            Assert.IsTrue(c_0 == "000000110000000000110000");
        }

        // Child Subtree Availability:
        Assert.IsTrue(subtree.ChildSubtreeAvailability == null);
    }

    [Test]
    public void ReadSubtreeTestLevel0_0_0()
    {
        // see https://github.com/CesiumGS/3d-tiles-samples/blob/main/1.1/SparseImplicitQuadtree/screenshot/subtreeInfo.md
        var subtreefile = File.OpenRead(@"testfixtures/0.0.0.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);
        Assert.IsTrue(subtree.SubtreeHeader.Magic == "subt");
        Assert.IsTrue(subtree.SubtreeHeader.Version == 1);
        Assert.IsTrue(subtree.SubtreeHeader.JsonByteLength== 312);
        Assert.IsTrue(subtree.SubtreeHeader.BinaryByteLength== 16);
        Assert.IsTrue(subtree.SubtreeBinary.Length == 16);

        // tile availability
        var t_0 = subtree.TileAvailability.AsString();
        Assert.IsTrue(t_0 == "101100000100110010000000");

        // content availability
        Assert.IsTrue(subtree.ContentAvailability == null);

        // Child Subtree Availability:
        if(subtree.ChildSubtreeAvailability != null)
        {
            var c_0 = subtree.ChildSubtreeAvailability.AsString();
            Assert.IsTrue(c_0 == "0000000000000000011000000000011001100000000001100000000000000000");

            var childSubtreeAvailability = BitArray2DCreator.GetBitArray2D(subtree.ChildSubtreeAvailability.AsString());
            var expectedSubtreeFiles = childSubtreeAvailability.GetAvailableFiles();
            Assert.IsTrue(expectedSubtreeFiles.Count() == 8);
        }
    }

}