namespace subtree
{
    public static class LevelOffset
    {
        public static int GetLevelOffset(int level)
        {
            return ((1 << (2 * level)) - 1) / 3;
        }

        public static int GetNumberOfLevels(string availability, bool isContentAvailability = false)
        {
            var level = 0;
            var length = availability.Length;
            var cont = true;

            while (cont)
            {
                var offset = GetLevelOffset(level);
                var offsetnext = GetLevelOffset(level + 1);

                if(offset<length && offsetnext > length)
                {
                    cont = false;
                }
                else
                {
                    var bits = availability.Substring(offset, offsetnext- offset);
                    var bitarray = BitArrayCreator.FromString(bits);
                    if (!isContentAvailability)
                    {
                        if (bitarray.Count(true) == 0)
                        {
                            cont = false;
                        }
                        else
                        {
                            level++;
                        }
                    }
                    else
                    {
                        level++;
                    }
                }
            }

            return level;
        }
    }
}
