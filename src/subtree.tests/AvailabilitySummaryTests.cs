using NUnit.Framework;

namespace subtree.tests;
public class AvailabilitySummaryTests
{
    [Test]
    public void AvailabilityTests()
    {
        var availability = "110010110000000000110000";
        var tileAvailability2d = BitArray2DCreator.GetBitArray2D(availability);

        var files = tileAvailability2d.GetAvailableFiles(3, 0, 5);

    }

    [Test]
    public void GetLevelTests()
    {
        var availability = "110010110000000000110000";
        // get level 1, bit 1..4 (1001)
        var level = Availability.GetLevel(availability, 1);

        Assert.That(level.Get(0,0));
        Assert.That(!level.Get(0, 1));
        Assert.That(level.Get(1, 1));
        Assert.That(!level.Get(1, 0));

    }

}
