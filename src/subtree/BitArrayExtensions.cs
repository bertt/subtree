using System.Collections;
using System.Text;

namespace subtree
{
    public static class BitArrayExtensions
    {
        public static string Write(this BitArray bitArray)
        {
            var sb = new StringBuilder();
            foreach (var b in bitArray)
            {
                sb.Append((bool)b ? "1" : "0");
            }
            return sb.ToString();
        }
    }
}
