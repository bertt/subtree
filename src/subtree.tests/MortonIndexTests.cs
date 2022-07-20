using NUnit.Framework;

namespace subtree.tests
{
    public class MortonIndexTests
    {
        [Test]
        public void MortonIndexTest()
        {
            // arrange
            var t = new Tile(0, 0, 0);
            t.Available = true;

            // act
            var mortonIndex = MortonIndex.GetMortonIndex(new List<Tile> { t });
            var mortonIndexBytes = MortonIndex.GetMortonIndexAsBytes(new List<Tile> { t });

            // assert
            Assert.IsTrue(mortonIndex == "1");
            Assert.IsTrue(mortonIndexBytes.Length == 1);
        }
    }
}
