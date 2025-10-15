using NUnit.Framework;

namespace subtree.tests;
public class AvailabilityLevel3DTests
{
    //[Test]
    public void AllLevelTests()
    {
        // arrange
        //var level0 = new AvailabilityLevel3D(0);
        var level1 = new AvailabilityLevel3D(1);
        level1.BitArray3D.Set(0, 1, 0, true);
        // level1.BitArray3D.Set(1, 0, 0, true);

        //Assert.That(level0.ToMortonIndex() == "0");
        var res = level1.ToMortonIndex();
        Assert.That(res == "01000000");
    }
}
