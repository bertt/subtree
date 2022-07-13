namespace subtree
{
    public static class LevelOffset
    {
        public static int GetLevelOffset(int level)
        {
            return ((1 << (2 * level)) - 1) / 3;
        }

        public static int GetNumberOfLevels(string availability)
        {
            var level = 0;
            var l = availability.Length;
            while (GetLevelOffset(level) < l)
            {
                level++;
            }

            return level - 1;
        }
    }
}
