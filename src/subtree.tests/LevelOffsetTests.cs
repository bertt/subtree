using NUnit.Framework;

namespace subtree.tests
{
    public class LevelOffsetTests
    {
        [Test]
        public void LevelOffsetTest()
        {
            Assert.IsTrue(LevelOffset.GetLevelOffset(0) == 0);
            Assert.IsTrue(LevelOffset.GetLevelOffset(1) == 1);
            Assert.IsTrue(LevelOffset.GetLevelOffset(2) == 5);
            Assert.IsTrue(LevelOffset.GetLevelOffset(3) == 21);
            Assert.IsTrue(LevelOffset.GetLevelOffset(4) == 85);
        }

        [Test]
        public void TileAvailabilityTests()
        {
            var contentavailability = "101100000100110010000000";
            var l = LevelOffset.GetNumberOfLevels(contentavailability);
            Assert.IsTrue(l==3);

            contentavailability = "11000000";
            l = LevelOffset.GetNumberOfLevels(contentavailability);
            Assert.IsTrue(l == 2);

            contentavailability = "10000000";
            l = LevelOffset.GetNumberOfLevels(contentavailability);
            Assert.IsTrue(l == 1);
        }
    }
}
