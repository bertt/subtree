using System.Collections;
using System.Text;

namespace subtree
{
    public class Subtree
    {
        public SubtreeHeader SubtreeHeader { get; set; } = null!;
        public string SubtreeJson { get; set; } = null!;
        public byte[] SubtreeBinary { get; set; } = null!;

        public List<BitArray>? ChildSubtreeAvailability { get; set; }
        public List<BitArray> TileAvailability { get; set; } = null!;
        public List<BitArray>? ContentAvailability { get; set; }

        public byte[] ToBytes()
        {
            var subtreeJsonPadded = BufferPadding.AddPadding(SubtreeJson);
            var subtreeBinaryPadded = BufferPadding.AddPadding(SubtreeBinary);

            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            SubtreeHeader.JsonByteLength  = (ulong)subtreeJsonPadded.Length;
            SubtreeHeader.BinaryByteLength = (ulong)subtreeBinaryPadded.Length;

            binaryWriter.Write(SubtreeHeader.AsBinary());
            binaryWriter.Write(Encoding.UTF8.GetBytes(subtreeJsonPadded));
            binaryWriter.Write(subtreeBinaryPadded);

            binaryWriter.Flush();
            binaryWriter.Close();
            return memoryStream.ToArray();
        }
    }
}
