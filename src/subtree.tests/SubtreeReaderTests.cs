using NUnit.Framework;
using Tedd;

namespace subtree.tests
{
    public class SubtreeReaderTests
    {
        [Test]
        public void MortonDecode()
        {
            // Take some numbers that illustrate well
            var x = (UInt32)0b00000000_00000000;
            var y = (UInt32)0b00000000_11111111;

            // Encode
            var result = MortonEncoding.Encode(x, y);

            // Test that result is now: 0b10101010_10101010
            var res = Convert.ToString(result, 2);
            Assert.True(res.Equals("1010101010101010"));
            //1010101010101010
            //1010101010101010

            // Decode
            MortonEncoding.Decode(result, out var xBack, out var yBack);

            // Test that we got back the same values as we started with
            //Assert.Equals(x, xBack);
            //Assert.Equals(y, yBack);
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
            }
        }

    }
}