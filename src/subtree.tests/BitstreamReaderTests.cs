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
            var t_0 = bitstreamTileAvailability[0].Write();
            Assert.IsTrue(t_0 == "10110000");
            var t_1 = bitstreamTileAvailability[1].Write();
            Assert.IsTrue(t_1 == "01001100");
            var t_2 = bitstreamTileAvailability[2].Write();
            Assert.IsTrue(t_2 == "10000000");
        }
    }
}
