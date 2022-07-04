using Newtonsoft.Json;
using NUnit.Framework;

namespace subtree.tests
{
    public class SubtreeWriterTests
    {
        [Test]
        public void TestWriteHeader()
        {
            var subtreefile = File.OpenRead(@"testfixtures/0.0.0.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);
            var header = subtree.SubtreeHeader;

            // act
            var headerBytes = subtree.ToBytes();
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
            var subtreeBytes = subtree.ToSubtreeBinary();

            // assert
            Assert.IsTrue(subtree.SubtreeBinary.Length == subtreeBytes.bytes.Length);
            Assert.IsTrue(Enumerable.SequenceEqual(subtree.SubtreeBinary, subtreeBytes.bytes));
            Assert.IsTrue(subtreeBytes.subtreeJson.buffers.First().byteLength == 16);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews.First().buffer== 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews.First().byteOffset == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews.First().byteLength == 3);

            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews[1].byteLength == 8);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews[1].buffer == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews[1].byteOffset == 8);
            Assert.IsTrue(subtreeBytes.subtreeJson.bufferViews[1].byteLength == 8);
            Assert.IsTrue(subtreeBytes.subtreeJson.tileAvailability.bitstream == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.tileAvailability.availableCount== 7);
            Assert.IsTrue(subtreeBytes.subtreeJson.contentAvailability[0].constant == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.contentAvailability[0].availableCount == 0);
            Assert.IsTrue(subtreeBytes.subtreeJson.childSubtreeAvailability.bitstream== 1);
            Assert.IsTrue(subtreeBytes.subtreeJson.childSubtreeAvailability.availableCount == 8);
        }
    }
}