using System.Collections;

namespace subtree
{
    public static class BitstreamReader
    {
        public static List<BitArray> Read(byte[] subtreeBinary, int offset, int length)
        {
            var slicedBytes = new Span<byte>(subtreeBinary).Slice(start: offset, length: length);
            var result = new List<BitArray>();
            foreach (var b in slicedBytes)
            {
                var bitArray = new BitArray(new byte[] { b });
                result.Add(bitArray);
            }

            return result;
        }

    }
}
