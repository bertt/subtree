namespace subtree;

public static class MortonOrder
{
    public static uint Encode3D(ulong x, ulong y, ulong z)
    {
        ulong mortonIndex = 0;

        for (int i = 0; i < (sizeof(ulong) * 8 / 3); i++) 
        {

            mortonIndex = SetBit(mortonIndex, 3 * i, GetBit(x, i));
            mortonIndex = SetBit(mortonIndex, 3 * i + 1, GetBit(y, i));
            mortonIndex = SetBit(mortonIndex, 3 * i + 2, GetBit(z, i));
        }

        return (uint)mortonIndex;
    }
    public static uint Encode2D(uint x, uint y)
    {
        ulong mortonIndex = 0;

        for (int i = 0; i < (sizeof(ulong) * 8 / 2); i++)
        {
            mortonIndex = SetBit(mortonIndex, 2 * i, GetBit(x, i));
            mortonIndex = SetBit(mortonIndex, 2 * i + 1, GetBit(y, i)); 
        }

        return (uint)mortonIndex;
    }

    public static (uint x, uint y) Decode2D(uint mortonIndex)
    {
        ulong x = 0, y = 0;

        for (int i = 0; i < (sizeof(ulong) * 8 / 2); i++)
        {
            x |= GetBit(mortonIndex, 2 * i) << i;
            y |= GetBit(mortonIndex, 2 * i + 1) << i;
        }

        return ((uint)x, (uint)y);
    }

    public static (uint x, uint y, uint z) Decode3D(uint mortonIndex)
    {
        ulong x = 0, y = 0, z = 0;

        for (int i = 0; i < (sizeof(ulong) * 8 / 3); i++)
        {
            x |= GetBit(mortonIndex, 3 * i) << i;
            y |= GetBit(mortonIndex, 3 * i + 1) << i;
            z |= GetBit(mortonIndex, 3 * i + 2) << i;
        }

        return ((uint)x, (uint)y, (uint)z);
    }

    static ulong GetBit(ulong value, int bitIndex)
    {
        return (value >> bitIndex) & 1;
    }

    static ulong SetBit(ulong value, int bitIndex, ulong bitValue)
    {
        return value | (bitValue << bitIndex);
    }
}
