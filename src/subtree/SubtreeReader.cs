using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace subtree
{
    public static class SubtreeReader
    {
        public static Subtree ReadSubtree(BinaryReader reader)
        {
            var subtreeHeader = new SubtreeHeader(reader);
            var subtreeJson = Encoding.UTF8.GetString(reader.ReadBytes(subtreeHeader.JsonByteLength));
            var subTreeBinary = reader.ReadBytes(subtreeHeader.BinaryByteLength);
            var subtree = new Subtree
            {
                SubtreeHeader = subtreeHeader,
                SubtreeJson = subtreeJson,
                SubtreeBinary = subTreeBinary
            };
            return subtree;
        }

        public static Subtree ReadSubtree(Stream stream)
        {
            using (var reader = new BinaryReader(stream))
            {
                var subtree = ReadSubtree(reader);
                return subtree;
            }
        }
    }
}
