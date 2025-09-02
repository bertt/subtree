using System.Collections;

namespace subtree
{
    public class BitArray3D
    {
        private BitArray bits;
        private int _dimension1;
        private int _dimension2;
        private int _dimension3;

        public BitArray3D(int dimension1, int dimension2, int dimension3)
        {
            _dimension1 = dimension1;
            _dimension2 = dimension2;
            _dimension3 = dimension3;
            bits = new BitArray(dimension1 * dimension2 *  dimension3);
        }

        private int GetIndex(int x, int y, int z)
        {
            return x * (_dimension2 * _dimension3) + y * _dimension3 + z;
        }

        public void Set(int x, int y, int z, bool value)
        {
            bits[GetIndex(x, y, z)] = value;
        }

        public bool Get(int x, int y, int z)
        {
            return bits[GetIndex(x, y, z)];
        }
    }
}
