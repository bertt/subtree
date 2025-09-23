namespace subtree;
public class AvailabilityLevel3D
{
    private int dimension;

    public AvailabilityLevel3D(int level)
    {
        Level = level;
        dimension = 1 << level;
        BitArray3D = new BitArray3D(dimension,dimension,dimension);
    }
    public int Level { get; set; }

    public BitArray3D BitArray3D { get; set; }

    public string ToMortonIndex()
    {
        var s = new char[dimension * dimension * dimension];
        for (var x = 0; x < dimension; x++)
        {
            for (var y = 0; y < dimension; y++)
            {
                for(var z=0; z< dimension; z++)
                {
                    var index = MortonOrder.Encode3D((uint)x, (uint)y, (uint)z);
                    s[index] = BitArray3D.Get(x, y, z) ? '1' : '0';
                }
            }
        }
        return new string(s);
    }

}
