namespace subtree
{
    public class AvailabilityLevels: List<AvailabilityLevel>
    {
    }

    public class AvailabilityLevel
    {
        public AvailabilityLevel (int level)
        {
            Level = level;
            var width = Level + 1;
            var height = Level + 1;
            BitArray2D = new BitArray2D(width, height);
        }
        public int Level { get; set; }

        public BitArray2D BitArray2D {get;set; }
    }
}
