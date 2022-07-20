using NUnit.Framework;

namespace subtree.tests
{
    public class AvailabilityLevelTests
    {
        [Test]
        public void AvailabilityLevelToMortonTest()
        {
            var s = Math.Sqrt(Math.Pow(4, 2));
            var availabilityLevels = new AvailabilityLevels();

            var availabilityLevel0 = new AvailabilityLevel(0);
            availabilityLevels.Add(availabilityLevel0);

            var availabilityLevel1 = new AvailabilityLevel(1);
            availabilityLevel1.BitArray2D.Set(0, 0, true);
            availabilityLevels.Add(availabilityLevel1);

            Assert.IsTrue(availabilityLevel1.ToMortonIndex() == "1000");
            Assert.IsTrue(availabilityLevels.ToMortonIndex() == "01000");
        }
    }
}
