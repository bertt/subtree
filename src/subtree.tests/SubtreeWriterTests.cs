using NUnit.Framework;

namespace subtree.tests
{
    public class SubtreeWriterTests
    {
        [Test]
        public void WriteSubtreeTestLevel0_0_0()
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
    }
}
