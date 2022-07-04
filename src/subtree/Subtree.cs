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
            // binaryWriter.Write(ToSubtreeBinary);

            binaryWriter.Flush();
            binaryWriter.Close();
            return memoryStream.ToArray();
        }


        public byte[] ToSubtreeBinary()
        {
            var bytes = new List<byte>();
            foreach (var tile in TileAvailability)
            {
                bytes.Add(tile.ToByte());
            }
            bytes = BufferPadding.AddBinaryPadding(bytes.ToArray()).ToList();

            if (ContentAvailability != null)
            {
                foreach (var content in ContentAvailability)
                {
                    bytes.Add(content.ToByte());
                }
                bytes = BufferPadding.AddPadding(bytes.ToArray()).ToList();
            }

            if (ChildSubtreeAvailability != null)
            {
                foreach (var childSubtree in ChildSubtreeAvailability)
                {
                    bytes.Add(childSubtree.ToByte());
                }
                bytes = BufferPadding.AddPadding(bytes.ToArray()).ToList();
            }


            var result = bytes.ToArray();
            return result;
        }

    }
}
