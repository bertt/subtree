using NUnit.Framework;

namespace subtree.tests
{
    public class BitstreamReaderTests
    {
        [Test]
        public void ReadBitstream() {

            var subtreefile = File.OpenRead(@"testfixtures/0.0.0.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);
            var bitstreamTileAvailability = BitstreamReader.Read(subtree.SubtreeBinary, 0, 3);
            var t_0 = bitstreamTileAvailability.AsString();
            Assert.IsTrue(t_0 == "101100000100110010000000");
        }
    }
}
