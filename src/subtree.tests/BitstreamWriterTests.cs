using NUnit.Framework;
using System.Collections;

namespace subtree.tests;

public class BitstreamWriterTests
{
    [Test]
    public void WriteBitstream()
    {
        var b0 = "10110000";
        BitArray bitArray = GetBitArray(b0);

        Assert.That(bitArray.Length == 8);
        Assert.That(bitArray[0] == true);
        Assert.That(bitArray[1] == false);
    }

    private static BitArray GetBitArray(string b0)
    {
        var chars = b0.ToCharArray();
        var res = new List<bool>();
        foreach (var char1 in chars)
        {
            res.Add(char1 == '0' ? false : true);
        }

        var bitArray = new BitArray(res.ToArray());
        return bitArray;
    }
}
