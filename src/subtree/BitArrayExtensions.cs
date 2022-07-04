using System.Collections;
using System.Text;

namespace subtree
{
    public static class BitArrayExtensions
    {
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
