using Newtonsoft.Json;
using System.Collections;
using System.Text;

namespace subtree
{
    public static class SubtreeReader
    {
        public static Subtree ReadSubtree(BinaryReader reader)
        {
            var subtreeHeader = new SubtreeHeader(reader);
            var subtreeJson = Encoding.UTF8.GetString(reader.ReadBytes((int)subtreeHeader.JsonByteLength));
            var subTreeBinary = reader.ReadBytes((int)subtreeHeader.BinaryByteLength);
            var subtree = new Subtree
            {
                SubtreeHeader = subtreeHeader,
                SubtreeJson = subtreeJson,
                SubtreeBinary = subTreeBinary
            };

            var subtreeJsonObject = JsonConvert.DeserializeObject<SubtreeJson>(subtree.SubtreeJson);
            if(subtreeJsonObject != null)
            {
                subtree.TileAvailability = ToBitstream(subtreeJsonObject.bufferViews[subtreeJsonObject.tileAvailability.bitstream], subtree.SubtreeBinary);

                var contentBitstream = subtreeJsonObject.contentAvailability.First().bitstream;
                if (contentBitstream!= null)
                {
                    subtree.ContentAvailability = ToBitstream(subtreeJsonObject.bufferViews[(int)contentBitstream], subtree.SubtreeBinary);
                }
                if (subtreeJsonObject.childSubtreeAvailability.bitstream != null)
                {
                    subtree.ChildSubtreeAvailability = ToBitstream(subtreeJsonObject.bufferViews[(int)subtreeJsonObject.childSubtreeAvailability.bitstream], subtree.SubtreeBinary);
                }
            }

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

        private static List<BitArray> ToBitstream(Bufferview bufferView, byte[] subtreeBinary)
        {
            var slicedBytes = new Span<byte>(subtreeBinary).Slice(start: bufferView.byteOffset, length: bufferView.byteLength);
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
