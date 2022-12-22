using NUnit.Framework;

namespace subtree.tests;

public class LevelTests
{
    [Test]
    public void AllLevelTests()
    {
        Assert.AreEqual(Level.GetLevel(1), 0);
        Assert.AreEqual(Level.GetLevel(4), 1);
        Assert.AreEqual(Level.GetLevel(16), 2);
        Assert.AreEqual(Level.GetLevel(64), 3);
        Assert.AreEqual(Level.GetLevel(256), 4);
        Assert.AreEqual(Level.GetLevel(1024), 5);
    }
}
