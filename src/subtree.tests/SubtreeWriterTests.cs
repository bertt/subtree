using Newtonsoft.Json;
using NUnit.Framework;

namespace subtree.tests;

public class SubtreeWriterTests
{
    [Test]
    public void TestWriteSubtreeRootwithConstants()
    {
        // create root subtree
        var subtree = new Subtree();

        subtree.TileAvailabilityConstant= 1;

        // act
        var bytes = SubtreeWriter.ToBytes(subtree);
        File.WriteAllBytes(@"test.subtree", bytes);
        var newSubtree = SubtreeReader.ReadSubtree(new MemoryStream(bytes));

        // assert
        Assert.That(newSubtree.TileAvailability == null);
        Assert.That(newSubtree.TileAvailabilityConstant == 1);
    }

    [Test]
    public void TestWriteSubtreeRootFile()
    {
        // arrange
        var file = @"testfixtures/SparseImplicitQuadtree/0.0.0.subtree";
        var subtreeBytes = File.ReadAllBytes(file);
        var subtreefile = File.OpenRead(file);
        var subtreeOriginal = SubtreeReader.ReadSubtree(subtreefile);

        // create root subtree
        var subtree = new Subtree();

        // tile availability (101100000100110010000000)
        var t0 = BitArrayCreator.FromString("101100000100110010000000");
        subtree.TileAvailability = t0;

        // subtree avaiability
        var c0 = BitArrayCreator.FromString("0000000000000000011000000000011001100000000001100000000000000000");
        subtree.ChildSubtreeAvailability =c0;

        // act
        var bytes = SubtreeWriter.ToBytes(subtree);
        var newSubtree = SubtreeReader.ReadSubtree(new MemoryStream(bytes));

        // assert
        Assert.That(bytes.Length == subtreeBytes.Length);
        Assert.That(subtreeOriginal.SubtreeHeader.Equals(newSubtree.SubtreeHeader));
        Assert.That(subtreeOriginal.SubtreeJson.Equals(newSubtree.SubtreeJson));
        Assert.That(Enumerable.SequenceEqual(bytes, subtreeBytes));
        Assert.That(Enumerable.SequenceEqual(subtreeOriginal.SubtreeBinary, newSubtree.SubtreeBinary));
    }

    [Test]
    public void TestWriteSubtreeLevel3File()
    {
        // arrange
        var file = @"testfixtures/SparseImplicitQuadtree/3.5.0.subtree";
        var subtreeBytes = File.ReadAllBytes(file);
        var subtreefile = File.OpenRead(file);
        var subtreeOriginal = SubtreeReader.ReadSubtree(subtreefile);

        // create root subtree
        var subtree = new Subtree();

        // tile availability
        var t0 = BitArrayCreator.FromString("110010110000000000110000");
        subtree.TileAvailability = t0;

        // content availability
        var c0 = BitArrayCreator.FromString("000000110000000000110000");
        subtree.ContentAvailability = c0;

        // act
        var bytes = SubtreeWriter.ToBytes(subtree);
        var newSubtree = SubtreeReader.ReadSubtree(new MemoryStream(bytes));

        // assert
        Assert.That(bytes.Length == subtreeBytes.Length);
        Assert.That(subtreeOriginal.SubtreeHeader.Equals(newSubtree.SubtreeHeader));
        Assert.That(subtreeOriginal.SubtreeJson.Equals(newSubtree.SubtreeJson));
        Assert.That(Enumerable.SequenceEqual(bytes, subtreeBytes));
        Assert.That(Enumerable.SequenceEqual(subtreeOriginal.SubtreeBinary, newSubtree.SubtreeBinary));
    }

    [Test]
    public void TestWriteHeader()
    {
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitQuadtree/0.0.0.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);
        var header = subtree.SubtreeHeader;

        // act
        var headerBytes = SubtreeWriter.ToBytes(subtree);
        var stream = new MemoryStream(headerBytes);
        var reader = new BinaryReader(stream);
        var newHeader = new SubtreeHeader(reader);

        // assert
        Assert.That(header.Magic == newHeader.Magic);
        Assert.That(header.Version == newHeader.Version);
        Assert.That(header.JsonByteLength == newHeader.JsonByteLength);
        Assert.That(header.BinaryByteLength == newHeader.BinaryByteLength);
    }

    [Test]
    public void TestWriteJson()
    {
        // arrange
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitQuadtree/0.0.0.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);
        var subtreeJson = GetSubtreeJson();

        // act
        var result = BufferPadding.AddPadding(JsonConvert.SerializeObject(subtreeJson, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        }));


        // assert
        Assert.That(subtree.SubtreeJson.Length == result.Length);
        Assert.That(subtree.SubtreeJson == result);
    }

    private static SubtreeJson GetSubtreeJson()
    {
        var subtreeJson = new SubtreeJson();
        subtreeJson.buffers = new List<Buffer>() { new Buffer() { byteLength = 16 } }.ToArray();
        subtreeJson.bufferViews = new List<Bufferview>() {
            new Bufferview(){ buffer = 0, byteOffset = 0, byteLength = 3 },
            new Bufferview(){ buffer = 0, byteOffset = 8, byteLength = 8 },
        }.ToArray();
        subtreeJson.tileAvailability = new Tileavailability() { bitstream = 0, availableCount = 7 };
        subtreeJson.contentAvailability = new List<Contentavailability>() {
            new Contentavailability(){constant = 0, availableCount = 00 }
            }.ToArray();
        subtreeJson.childSubtreeAvailability = new Childsubtreeavailability() { bitstream = 1, availableCount = 8 };
        return subtreeJson;
    }

    [Test]
    public void TestWriteBinary()
    {
        // arrange
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitQuadtree/0.0.0.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);

        // act
        var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

        // assert
        Assert.That(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
        Assert.That(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
        Assert.That(subtreeBytes.subtreeJson.buffers.First().byteLength == 16);
        Assert.That(subtreeBytes.subtreeJson.bufferViews.First().buffer == 0);
        Assert.That(subtreeBytes.subtreeJson.bufferViews.First().byteOffset == 0);
        Assert.That(subtreeBytes.subtreeJson.bufferViews.First().byteLength == 3);

        Assert.That(subtreeBytes.subtreeJson.bufferViews[1].byteLength == 8);
        Assert.That(subtreeBytes.subtreeJson.bufferViews[1].buffer == 0);
        Assert.That(subtreeBytes.subtreeJson.bufferViews[1].byteOffset == 8);
        Assert.That(subtreeBytes.subtreeJson.bufferViews[1].byteLength == 8);
        Assert.That(subtreeBytes.subtreeJson.tileAvailability.bitstream == 0);
        Assert.That(subtreeBytes.subtreeJson.tileAvailability.availableCount == 7);
        Assert.That(subtreeBytes.subtreeJson.contentAvailability[0].constant == 0);
        Assert.That(subtreeBytes.subtreeJson.contentAvailability[0].availableCount == 0);
        Assert.That(subtreeBytes.subtreeJson.childSubtreeAvailability.bitstream == 1);
        Assert.That(subtreeBytes.subtreeJson.childSubtreeAvailability.availableCount == 8);
    }

    [Test]
    public void TestWriteBinary1()
    {
        // arrange
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitQuadtree/3.4.1.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);

        // act
        var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

        // assert
        Assert.That(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
        Assert.That(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
    }

    [Test]
    public void TestWriteBinary2()
    {
        // arrange
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitQuadtree/3.5.0.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);

        // act
        var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

        // assert
        Assert.That(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
        Assert.That(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
    }


    [Test]
    public void TestWriteBinary3()
    {
        // arrange
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitQuadtree/3.6.3.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);

        // act
        var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

        // assert
        Assert.That(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
        Assert.That(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
    }

    [Test]
    public void TestWriteBinary4()
    {
        // arrange
        var subtreefile = File.OpenRead(@"testfixtures/SparseImplicitQuadtree/3.7.2.subtree");
        var subtree = SubtreeReader.ReadSubtree(subtreefile);

        // act
        var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

        // assert
        Assert.That(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
        Assert.That(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
    }

    [Test]
    public void SimpleSubtreeFile()
    {
        var tiles = new List<Tile>() { new Tile(0, 0, 0) };

        var subtree_root = new Subtree();
        var tileavailiability = BitArrayCreator.FromString("1");

        subtree_root.TileAvailability = tileavailiability;

        var s0_root = BitArrayCreator.FromString("1");
        subtree_root.ContentAvailability = s0_root;

        var subtreebytes = SubtreeWriter.ToBytes(subtree_root);

        File.WriteAllBytes(@"0_0_0.subtree", subtreebytes);
    }


    [Test]
    public void SimpleSubtreeFileWithContentNull()
    {
        var tiles = new List<Tile>() { new Tile(0, 0, 0) };

        var subtree_root = new Subtree();
        var tileavailiability = BitArrayCreator.FromString("1");

        subtree_root.TileAvailability = tileavailiability;

        var s0_root = BitArrayCreator.FromString("1");
        subtree_root.ContentAvailability = null;
        // subtree_root.ChildSubtreeAvailability = "1000";
        var childSubtreeAvailability = "1000";

        var subtreebytes = SubtreeWriter.ToBytes("11000", subtreeAvailability:childSubtreeAvailability);

        File.WriteAllBytes(@"0_0_0.subtree", subtreebytes);
    }


    [Test]
    public void SimpleSubtreeFileWriter()
    {
        var subtreeBytes = SubtreeWriter.ToBytes("1", "1");
        Assert.That(subtreeBytes.Length > 0);
    }
}