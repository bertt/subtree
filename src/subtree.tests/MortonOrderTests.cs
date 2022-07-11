using NUnit.Framework;
using System.Collections;

namespace subtree.tests
{
    public class MortonOrderTests
    {
        [Test]
        public void MortonRoundTripTest()
        {
            // sample from: https://github.com/CesiumGS/3d-tiles/blob/draft-1.1/specification/ImplicitTiling/AVAILABILITY.adoc#implicittiling-availability-indexing
            var mortonIndex = (uint)0b010011;
            var res = MortonOrder.Decode2D(mortonIndex);
            Assert.IsTrue(res.x == 5 && res.y == 1);
            Assert.IsTrue(MortonOrder.Encode2D(res.x, res.y) == mortonIndex);
        }

        [Test]
        public void MortonEncode2DTests()
        {
            // uint p = 0;
            // var p1 = new BitArray(new uint[] {0});
            Assert.IsTrue(MortonOrder.Encode2D(0, 0) == 0); // binary: 000
            Assert.IsTrue(MortonOrder.Encode2D(1, 0) == 1); // binary: 001
            Assert.IsTrue(MortonOrder.Encode2D(0, 1) == 2); // binary: 010
            Assert.IsTrue(MortonOrder.Encode2D(1, 1) == 3); // binary: 011
            Assert.IsTrue(MortonOrder.Encode2D(2, 0) == 4); // binary: 100
            Assert.IsTrue(MortonOrder.Encode2D(3, 0) == 5); // binary: 101
            Assert.IsTrue(MortonOrder.Encode2D(2, 1) == 6); // binary: 110
            Assert.IsTrue(MortonOrder.Encode2D(3, 1) == 7); // binary: 111
            Assert.IsTrue(MortonOrder.Encode2D(7, 5) == 55); // binary: 110111
            Assert.IsTrue(MortonOrder.Encode2D(65535, 65535) == 4294967295); // binary: 11111111111111111111111111111111
            Assert.IsTrue(MortonOrder.Encode2D(65535, 0) == 1431655765);  //// binary:  1010101010101010101010101010101
            Assert.IsTrue(MortonOrder.Encode2D(0, 65535) == 2863311530);  //// binary:  10101010101010101010101010101010

            Assert.IsTrue(MortonOrder.Encode2D(0, 65535) == 2863311530);  //// binary:  10101010101010101010101010101010

        }

        [Test]
        public void MortonDecode2DTests()
        {
            Assert.IsTrue(MortonOrder.Decode2D(0) == (0, 0)); // binary: 000
            Assert.IsTrue(MortonOrder.Decode2D(1) == (1, 0)); // binary: 001
            Assert.IsTrue(MortonOrder.Decode2D(2) == (0, 1)); // binary: 010
            Assert.IsTrue(MortonOrder.Decode2D(3) == (1, 1)); // binary: 011
            Assert.IsTrue(MortonOrder.Decode2D(4) == (2, 0)); // binary: 100
            Assert.IsTrue(MortonOrder.Decode2D(5) == (3, 0)); // binary: 101
            Assert.IsTrue(MortonOrder.Decode2D(6) == (2, 1)); // binary: 110
            Assert.IsTrue(MortonOrder.Decode2D(7) == (3, 1)); // binary: 111
            Assert.IsTrue(MortonOrder.Decode2D(55) == (7, 5)); // binary: 110111
            Assert.IsTrue(MortonOrder.Decode2D(4294967295) == (65535, 65535)); // binary: 11111111111111111111111111111111
            Assert.IsTrue(MortonOrder.Decode2D(1431655765) == (65535, 0)); // binary:  1010101010101010101010101010101
            Assert.IsTrue(MortonOrder.Decode2D(2863311530) == (0, 65535)); // binary: 10101010101010101010101010101010
        }
    }
}
