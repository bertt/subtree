using Newtonsoft.Json;
using NUnit.Framework;
using Tedd;

namespace subtree.tests
{
    public class SubtreeWriterTests
    {
        [Test]
        public void TestMorton()
        {
            // Take some numbers that illustrate well
            var x = (uint)0b0000000000000000;
            var y = (uint)0b0000000011111111;

            // Encode
            var result = MortonEncoding.Encode(x, y);

            // Test that result is now: 0b10101010_10101010
            Assert.AreEqual("1010101010101010", Convert.ToString(result, 2));

            // Decode
            MortonEncoding.Decode(result, out var xBack, out var yBack);

            // 101100000100110010000000
            MortonEncoding.Decode((uint)0b0111, out var xBack1, out var yBack1);


            // Test that we got back the same values as we started with
            Assert.AreEqual(x, xBack);
            Assert.AreEqual(y, yBack);
        }

        [Test]
        public void TestWriteSubtreeRootwithConstants()
        {
            // create root subtree
            var subtree = new Subtree();

            subtree.TileAvailabiltyConstant= 1;

            // act
            var bytes = SubtreeWriter.ToBytes(subtree);
            File.WriteAllBytes(@"d:\aaa\test.subtree", bytes);
            var newSubtree = SubtreeReader.ReadSubtree(new MemoryStream(bytes));

            // assert
            Assert.IsTrue(newSubtree.TileAvailability == null);
            Assert.IsTrue(newSubtree.TileAvailabiltyConstant == 1);
        }

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
            var t0 = BitArrayCreator.FromString("110010110000000000110000");
            subtree.TileAvailability = t0;

            // content availability
            var c0 = BitArrayCreator.FromString("000000110000000000110000");
            subtree.ContentAvailability = c0;

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