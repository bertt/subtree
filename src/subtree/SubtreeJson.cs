namespace subtree
{
    public class SubtreeJson
    {
        public Buffer[] buffers { get; set; }
        public Bufferview[] bufferViews { get; set; }
        public Tileavailability tileAvailability { get; set; }
        public Contentavailability[] contentAvailability { get; set; }
        public Childsubtreeavailability childSubtreeAvailability { get; set; }
    }

}
