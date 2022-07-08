using System.Collections;
using System.Text;

namespace subtree
{
    public static class BitArrayExtensions
    {
        public static BitArray Append(this BitArray current, BitArray after)
        {
            var bools = new bool[current.Count + after.Count];
            current.CopyTo(bools, 0);
            after.CopyTo(bools, current.Count);
            return new BitArray(bools);
        }
        public static uint ToUint(this BitArray input)
        {
            uint result = 0;
            uint bitCount = 1;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i]) result |= bitCount;

                bitCount = bitCount << 1;
            }

            return result;
        }
        public static string AsString(this BitArray bitArray)
        {
            var sb = new StringBuilder();
            foreach (var b in bitArray)
            {
                sb.Append((bool)b ? "1" : "0");
            }
            return sb.ToString();
        }

        public static byte ToByte(this BitArray bitArray)
        {
            if (bitArray.Count != 8)
            {
                throw new ArgumentException("bits");
            }
            byte[] bytes = new byte[1];
            bitArray.CopyTo(bytes, 0);
            return bytes[0];
        }

        public static int Count(this BitArray bitArray, bool whereClause= false)
        {
            return (from bool m in bitArray where m == whereClause select m).Count();
        }

    }
}
