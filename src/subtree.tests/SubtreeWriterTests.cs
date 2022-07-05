using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.Collections;

namespace subtree.tests
{
    public class SubtreeWriterTests
    {
        [Test]
        public void TestWriteSubtreeRootFile()
        {
            // arrange
            var file = @"testfixtures/0.0.0.subtree";
            var subtreeBytes = File.ReadAllBytes(file);
            var subtreefile = File.OpenRead(file);
            var subtreeOriginal = SubtreeReader.ReadSubtree(subtreefile);

            // create root subtree
            var subtree = new Subtree();

            // tile availability
            var t0 = BitArrayCreator.FromString("10110000");
            var t1 = BitArrayCreator.FromString("01001100");
            var t2 = BitArrayCreator.FromString("10000000");
            subtree.TileAvailability = new List<BitArray>() { t0, t1, t2 };

            // subtree avaiability
            var c0 = BitArrayCreator.FromString("00000000");
            var c1 = BitArrayCreator.FromString("00000000");
            var c2 = BitArrayCreator.FromString("01100000");
            var c3 = BitArrayCreator.FromString("00000110");
            var c4 = BitArrayCreator.FromString("01100000");
            var c5 = BitArrayCreator.FromString("00000110");
            var c6 = BitArrayCreator.FromString("00000000");
            var c7 = BitArrayCreator.FromString("00000000");

            subtree.ChildSubtreeAvailability = new List<BitArray>() { c0, c1, c2, c3, c4, c5, c6, c7 };

            // act
            var bytes = SubtreeWriter.ToBytes(subtree);
            var newSubtree = SubtreeReader.ReadSubtree(new MemoryStream(bytes));

            // assert
            Assert.IsTrue(bytes.Length == subtreeBytes.Length);
            Assert.IsTrue(subtreeOriginal.SubtreeHeader.Equals(newSubtree.SubtreeHeader));
            Assert.IsTrue(subtreeOriginal.SubtreeJson.Equals(newSubtree.SubtreeJson));
            Assert.IsTrue(Enumerable.SequenceEqual(bytes, subtreeBytes));
            Assert.IsTrue(Enumerable.SequenceEqual(subtreeOriginal.SubtreeBinary, newSubtree.SubtreeBinary));
        }


        [Test]
        public void TestWriteSubtreeLevel3File()
        {
            // arrange
            var file = @"testfixtures/3.5.0.subtree";
            var subtreeBytes = File.ReadAllBytes(file);
            var subtreefile = File.OpenRead(file);
            var subtreeOriginal = SubtreeReader.ReadSubtree(subtreefile);

            // create root subtree
            var subtree = new Subtree();

            // tile availability
            var t0 = BitArrayCreator.FromString("11001011");
            var t1 = BitArrayCreator.FromString("00000000");
            var t2 = BitArrayCreator.FromString("00110000");
            subtree.TileAvailability = new List<BitArray>() { t0, t1, t2 };

            // subtree avaiability
            var c0 = BitArrayCreator.FromString("00000011");
            var c1 = BitArrayCreator.FromString("00000000");
            var c2 = BitArrayCreator.FromString("00110000");

            subtree.ContentAvailability = new List<BitArray>() { c0, c1, c2};

            // act
            var bytes = SubtreeWriter.ToBytes(subtree);
            var newSubtree = SubtreeReader.ReadSubtree(new MemoryStream(bytes));

            // assert
            Assert.IsTrue(bytes.Length == subtreeBytes.Length);
            Assert.IsTrue(subtreeOriginal.SubtreeHeader.Equals(newSubtree.SubtreeHeader));
            Assert.IsTrue(subtreeOriginal.SubtreeJson.Equals(newSubtree.SubtreeJson));
            Assert.IsTrue(Enumerable.SequenceEqual(bytes, subtreeBytes));
            Assert.IsTrue(Enumerable.SequenceEqual(subtreeOriginal.SubtreeBinary, newSubtree.SubtreeBinary));
        }

        [Test]
        public void TestWriteHeader()
        {
            var subtreefile = File.OpenRead(@"testfixtures/0.0.0.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);
            var header = subtree.SubtreeHeader;

            // act
            var headerBytes = SubtreeWriter.ToBytes(subtree);
            var stream = new MemoryStream(headerBytes);
            var reader = new BinaryReader(stream);
            var newHeader = new SubtreeHeader(reader);

            // assert
            Assert.IsTrue(header.Magic == newHeader.Magic);
            Assert.IsTrue(header.Version == newHeader.Version);
            Assert.IsTrue(header.JsonByteLength == newHeader.JsonByteLength);
            Assert.IsTrue(header.BinaryByteLength == newHeader.BinaryByteLength);
        }

        [Test]
        public void TestWriteJson()
        {
            // arrange
            var subtreefile = File.OpenRead(@"testfixtures/0.0.0.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);
            var subtreeJson = GetSubtreeJson();

            // act
            var result = BufferPadding.AddPadding(JsonConvert.SerializeObject(subtreeJson, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));


            // assert
            Assert.IsTrue(subtree.SubtreeJson.Length == result.Length);
            Assert.IsTrue(subtree.SubtreeJson == result);
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
            var subtreefile = File.OpenRead(@"testfixtures/0.0.0.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);

            // act
            var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

            // assert
            Assert.IsTrue(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
            Assert.IsTrue(subtreeBytes.subtreeJson.buffers.First().byteLength == 16);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews.First().buffer == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews.First().byteOffset == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews.First().byteLength == 3);

            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews[1].byteLength == 8);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews[1].buffer == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews[1].byteOffset == 8);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews[1].byteLength == 8);
            Assert.IsTrue(subtreeBytes.subtreeJson.tileAvailability.bitstream == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.tileAvailability.availableCount == 7);
            Assert.IsTrue(subtreeBytes.subtreeJson.contentAvailability[0].constant == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.contentAvailability[0].availableCount == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.childSubtreeAvailability.bitstream == 1);
            Assert.IsTrue(subtreeBytes.subtreeJson.childSubtreeAvailability.availableCount == 8);
        }

        [Test]
        public void TestWriteBinary1()
        {
            // arrange
            var subtreefile = File.OpenRead(@"testfixtures/3.4.1.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);

            // act
            var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

            // assert
            Assert.IsTrue(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
        }

        [Test]
        public void TestWriteBinary2()
        {
            // arrange
            var subtreefile = File.OpenRead(@"testfixtures/3.5.0.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);

            // act
            var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

            // assert
            Assert.IsTrue(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
        }


        [Test]
        public void TestWriteBinary3()
        {
            // arrange
            var subtreefile = File.OpenRead(@"testfixtures/3.6.3.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);

            // act
            var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

            // assert
            Assert.IsTrue(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
        }

        [Test]
        public void TestWriteBinary4()
        {
            // arrange
            var subtreefile = File.OpenRead(@"testfixtures/3.7.2.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);

            // act
            var subtreeBytes = SubtreeWriter.ToSubtreeBinary(subtree);

            // assert
            Assert.IsTrue(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
        }
    }
}