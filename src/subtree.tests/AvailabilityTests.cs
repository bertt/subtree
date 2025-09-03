using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subtree.tests;
public class AvailabilityTests
{
    [Test]
    public void TestAvailabilityLevelQuadreee()
    {
        var bitArray2D = Availability.GetLevel("11000", 0);
        Assert.That(bitArray2D.GetWidth() == 1);
        Assert.That(bitArray2D.GetHeight() == 1);
        Assert.That(bitArray2D.Get(0, 0));

        var bitArray2D_ = Availability.GetLevel("11000", 1);
        Assert.That(bitArray2D_.GetWidth() == 2);
        Assert.That(bitArray2D_.GetHeight() == 2);
        Assert.That(bitArray2D_.Get(0, 0));
    }


    [Test]
    public void TestAvailabilityLevelOctree()
    {
        var bitArray2D = Availability.GetLevel("1", 0);
        Assert.That(bitArray2D.GetWidth() == 1);
        Assert.That(bitArray2D.GetHeight() == 1);
        Assert.That(bitArray2D.Get(0, 0));

        var bitArray3D = Availability.GetLevel3D("110000000", 1);
        Assert.That(bitArray3D.GetDimension() == 2);
        Assert.That(bitArray3D.Get(0,0,0));
    }

}
