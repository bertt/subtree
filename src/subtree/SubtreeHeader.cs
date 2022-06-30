using System.Text;

namespace subtree
{
    public class SubtreeHeader
    {
        public SubtreeHeader(BinaryReader reader)
        {
            Magic = Encoding.UTF8.GetString(reader.ReadBytes(4));
            Version = (int)reader.ReadUInt32();
            JsonByteLength = (int)reader.ReadUInt64();
            BinaryByteLength = (int)reader.ReadUInt64();
        }
        public string Magic { get; set; }
        public int Version { get; set; }
        public int JsonByteLength { get; set; }
        public int BinaryByteLength { get; set; }
    }
}


