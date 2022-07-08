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
            var sum = subtree.TileAvailability[0].Append(subtree.TileAvailability[1]);
            Assert.IsTrue(subtree.TileAvailability.Count == 3);
            var t_0 = subtree.TileAvailability[0].AsString();
            Assert.IsTrue(t_0 == "11001011");
            var t_1 = subtree.TileAvailability[1].AsString();
            Assert.IsTrue(t_1 == "00000000");
            var t_2 = subtree.TileAvailability[2].AsString();
            Assert.IsTrue(t_2 == "00110000");

            // content availability
            if (subtree.ContentAvailability != null){
                Assert.IsTrue(subtree.ContentAvailability.Count == 3);
                var c_0 = subtree.ContentAvailability[0].AsString();
                Assert.IsTrue(c_0 == "00000011");
                var c_1 = subtree.ContentAvailability[1].AsString();
                Assert.IsTrue(c_1 == "00000000");
                var c_2 = subtree.ContentAvailability[2].AsString();
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
            var t_0 = subtree.TileAvailability[0].AsString();
            Assert.IsTrue(t_0 == "10110000");
            var t_1 = subtree.TileAvailability[1].AsString();
            Assert.IsTrue(t_1 == "01001100");
            var t_2 = subtree.TileAvailability[2].AsString();
            Assert.IsTrue(t_2 == "10000000");

            // content availability
            Assert.IsTrue(subtree.ContentAvailability == null);

            // Child Subtree Availability:
            if(subtree.ChildSubtreeAvailability != null)
            {
                var c_0 = subtree.ChildSubtreeAvailability[0].AsString();
                Assert.IsTrue(c_0 == "00000000");
                var c_1 = subtree.ChildSubtreeAvailability[1].AsString();
                Assert.IsTrue(c_1 == "00000000");
                var c_2 = subtree.ChildSubtreeAvailability[2].AsString();
                Assert.IsTrue(c_2 == "01100000");
                var c_3 = subtree.ChildSubtreeAvailability[3].AsString();
                Assert.IsTrue(c_3 == "00000110");
                var c_4 = subtree.ChildSubtreeAvailability[4].AsString();
                Assert.IsTrue(c_4 == "01100000");
                var c_5 = subtree.ChildSubtreeAvailability[5].AsString();
                Assert.IsTrue(c_5 == "00000110");
                var c_6 = subtree.ChildSubtreeAvailability[6].AsString();
                Assert.IsTrue(c_6 == "00000000");
                var c_7 = subtree.ChildSubtreeAvailability[7].AsString();
                Assert.IsTrue(c_7 == "00000000");
            }
        }

    }
}