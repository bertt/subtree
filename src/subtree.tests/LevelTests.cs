using NUnit.Framework;

namespace subtree.tests;

public class LevelTests
{
    [Test]
    public void AllLevelTests()
    {
        Assert.That(0, Is.EqualTo(Level.GetLevel(1)));
        Assert.That(1, Is.EqualTo(Level.GetLevel(4)));
        Assert.That(2, Is.EqualTo(Level.GetLevel(16)));
        Assert.That(3, Is.EqualTo(Level.GetLevel(64)));
        Assert.That(4, Is.EqualTo(Level.GetLevel(256)));
        Assert.That(5, Is.EqualTo(Level.GetLevel(1024)));
    }
}
