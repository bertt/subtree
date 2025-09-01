using System;
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




    }
}
