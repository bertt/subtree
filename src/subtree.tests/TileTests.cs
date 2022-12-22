using NUnit.Framework;

namespace subtree.tests;

public class TileTests
{
    [Test]
    public void TileFirstTest()
    {
        var t = new Tile(0, 0, 0, true);
        Assert.IsTrue(t.Children().Count == 4);
    }
}
