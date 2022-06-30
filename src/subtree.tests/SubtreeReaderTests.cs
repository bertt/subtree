using Newtonsoft.Json;
using NUnit.Framework;

namespace subtree.tests
{
    public class SubtreeReaderTests
    {
        Stream subtreefile;

        [SetUp]
        public void Setup()
        {
            subtreefile = File.OpenRead(@"testfixtures/0.0.0.subtree");
            Assert.IsTrue(subtreefile != null);
        }

        [Test]
        public void ReadSubtreeTest()
        {
            var subtree = SubtreeReader.ReadSubtree(subtreefile);
            Assert.IsTrue(subtree.SubtreeHeader.Magic == "subt");
            Assert.IsTrue(subtree.SubtreeHeader.Version == 1);
            Assert.IsTrue(subtree.SubtreeHeader.JsonByteLength== 312);
            Assert.IsTrue(subtree.SubtreeHeader.BinaryByteLength== 16);
            // {\"buffers\":[{\"byteLength\":16}],\"bufferViews\":[{\"buffer\":0,\"byteOffset\":0,\"byteLength\":3},{\"buffer\":0,\"byteOffset\":8,\"byteLength\":8}],\"tileAvailability\":{\"bitstream\":0,\"availableCount\":7},\"contentAvailability\":[{\"availableCount\":0,\"constant\":0}],\"childSubtreeAvailability\":{\"bitstream\":1,\"availableCount\":8}}     

            var subtreeJson = JsonConvert.DeserializeObject<SubtreeJson>(subtree.SubtreeJson);
            if(subtreeJson != null)
            {
                Assert.IsTrue(subtreeJson.buffers.First().byteLength == 16);
            }
        }
    }
}