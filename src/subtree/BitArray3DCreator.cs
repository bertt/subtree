namespace subtree;
public class BitArray3DCreator
{
    public static BitArray3D GetBitArray3D(string availability)
    {
        var width = GetWidth(availability);
        var result = new BitArray3D(width, width, width);

        var bitarray = BitArrayCreator.FromString(availability);
        for (uint x = 0; x < width; x++)
        {
            for (uint y = 0; y < width; y++)
            {
                for(uint z=0;z < width; z++)
                {
                    var mortonIndex3D = MortonOrder.Encode3D(x, y, z);
                    var cel3D = bitarray.Get((int)mortonIndex3D);
                    result.Set((int)x, (int)y, (int)z, cel3D);
                }
            }

        }
        return result;
    }

    private static int GetWidth(string mortonIndex)
    {
        var length = mortonIndex.Length;
        // do for octree
        double size = Math.Cbrt(length);
        return (int)size;
    }

}
