using NUnit.Framework;
using subtree;

namespace subtree.tests
{
    public class AvailabilityTests
    {
        public void geMaxLevelTest()
        {
            var bl = new Tile(1, 0, 0, true);
            var br = new Tile(1, 1, 0, true);
            var ul = new Tile(1, 0, 1, true);
            var ur = new Tile(1, 1, 1, false);
            var ur1 = new Tile(2, 1, 1, true);


            var tiles = new List<Tile> { bl, br, ul, ur, ur1 };

            var res = GetMaxLevel(tiles);
            Assert.IsTrue(res == 2);
        }

        private int GetMaxLevel(List<Tile> tiles)
        {
            return tiles.Max(x => x.Z);
        }
    }
}
