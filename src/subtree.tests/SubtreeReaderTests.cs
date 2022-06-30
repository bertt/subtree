using NUnit.Framework;
using System.Collections;
using System.Text;

namespace subtree.tests
{
    public class SubtreeReaderTests
    {
        [Test]
        public void ReadSubtreeTestLevel3_5_0()
        {
            // see https://github.com/CesiumGS/3d-tiles-samples/blob/main/1.1/SparseImplicitQuadtree/screenshot/subtreeInfo.md
            var subtreefile = File.OpenRead(@"testfixtures/3.5.0.subtree");
            var subtree = SubtreeReader.ReadSubtree(subtreefile);

            // tile availability
            Assert.IsTrue(subtree.TileAvailability.Count == 3);
            var t_0 = GetBits(subtree.TileAvailability[0]);
            Assert.IsTrue(t_0 == "11001011");
            var t_1 = GetBits(subtree.TileAvailability[1]);
            Assert.IsTrue(t_1 == "00000000");
            var t_2 = GetBits(subtree.TileAvailability[2]);
            Assert.IsTrue(t_2 == "00110000");

            // content availability
            if (subtree.ContentAvailability != null){
                Assert.IsTrue(subtree.ContentAvailability.Count == 3);
                var c_0 = GetBits(subtree.ContentAvailability[0]);
                Assert.IsTrue(c_0 == "00000011");
                var c_1 = GetBits(subtree.ContentAvailability[1]);
                Assert.IsTrue(c_1 == "00000000");
                var c_2 = GetBits(subtree.ContentAvailability[2]);
                Assert.IsTrue(c_2 == "00110000");
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
            var t_0 = GetBits(subtree.TileAvailability[0]);
            Assert.IsTrue(t_0 == "10110000");
            var t_1 = GetBits(subtree.TileAvailability[1]);
            Assert.IsTrue(t_1 == "01001100");
            var t_2 = GetBits(subtree.TileAvailability[2]);
            Assert.IsTrue(t_2 == "10000000");

            // content availability
            Assert.IsTrue(subtree.ContentAvailability == null);

            // Child Subtree Availability:
            if(subtree.ChildSubtreeAvailability != null)
            {
                var c_0 = GetBits(subtree.ChildSubtreeAvailability[0]);
                Assert.IsTrue(c_0 == "00000000");
                var c_1 = GetBits(subtree.ChildSubtreeAvailability[1]);
                Assert.IsTrue(c_1 == "00000000");
                var c_2 = GetBits(subtree.ChildSubtreeAvailability[2]);
                Assert.IsTrue(c_2 == "01100000");
                var c_3 = GetBits(subtree.ChildSubtreeAvailability[3]);
                Assert.IsTrue(c_3 == "00000110");
                var c_4 = GetBits(subtree.ChildSubtreeAvailability[4]);
                Assert.IsTrue(c_4 == "01100000");
                var c_5 = GetBits(subtree.ChildSubtreeAvailability[5]);
                Assert.IsTrue(c_5 == "00000110");
                var c_6 = GetBits(subtree.ChildSubtreeAvailability[6]);
                Assert.IsTrue(c_6 == "00000000");
                var c_7 = GetBits(subtree.ChildSubtreeAvailability[7]);
                Assert.IsTrue(c_7 == "00000000");
            }
        }

        private static string GetBits(BitArray bits)
        {
            var sb = new StringBuilder();
            foreach (var b in bits)
            {
                sb.Append((bool)b ? "1" : "0");
            }
            return sb.ToString();
        }
    }
}