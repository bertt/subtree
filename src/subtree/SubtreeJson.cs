namespace subtree;

public record SubtreeJson
{
    public Buffer[] buffers { get; set; } = null!;
    public Bufferview[] bufferViews { get; set; } = null!;
    public Tileavailability tileAvailability { get; set; } = null!;
    public Contentavailability[] contentAvailability { get; set; } = null!;
    public Childsubtreeavailability childSubtreeAvailability { get; set; } = null!;
    public SubtreeMetadata subtreeMetadata { get; set; } = null!;
}



public class Properties
{
    public string author { get; set; }
    public string date { get; set; }
    public string[] credits { get; set; }
    public int Height { get; set; }
}
