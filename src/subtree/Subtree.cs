using System.Collections;
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
            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            binaryWriter.Write(SubtreeHeader.AsBinary());
            binaryWriter.Flush();
            binaryWriter.Close();
            return memoryStream.ToArray();
        }
    }
}
