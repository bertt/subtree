using System.Collections;

namespace subtree
{
    public record Subtree
    {
        public Subtree()
        {
            SubtreeHeader = new SubtreeHeader();
        }

        public SubtreeHeader SubtreeHeader { get; set; } = null!;
        public string SubtreeJson { get; set; } = null!;
        public byte[] SubtreeBinary { get; set; } = null!;

        public List<BitArray>? ChildSubtreeAvailability { get; set; }
        public List<BitArray> TileAvailability { get; set; } = null!;
        public List<BitArray>? ContentAvailability { get; set; }
    }
}
