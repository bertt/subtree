using System.Collections;

namespace subtree;

public record Subtree
{
    public Subtree()
    {
        SubtreeHeader = new SubtreeHeader();
        TileAvailabiltyConstant = 0;
        ContentAvailabiltyConstant = 0;
    }

    public SubtreeHeader SubtreeHeader { get; set; } = null!;
    public string SubtreeJson { get; set; } = null!;
    public byte[] SubtreeBinary { get; set; } = null!;

    public BitArray? ChildSubtreeAvailability { get; set; }
    public BitArray TileAvailability { get; set; } = null!;

    public int TileAvailabiltyConstant { get; set; }

    public BitArray? ContentAvailability { get; set; } = null!;

    public int ContentAvailabiltyConstant { get; set; }
}
