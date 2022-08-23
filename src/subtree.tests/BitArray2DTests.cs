using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subtree.tests
{
    public class BitArray2DTests
    {
        [Test]
        public void IsAvailableTest()
        {
            var ba = new BitArray2D(2, 2);
            ba.Set(0, 0, true);

            Assert.IsTrue(ba.IsAvailable());

            ba.Set(0, 0, false);
            Assert.IsFalse(ba.IsAvailable());

        }
    }
}
