namespace subtree
{
    public static class LevelOffset
    {
        public static int GetLevelOffset(int level)
        {
            return ((1 << (2 * level)) - 1) / 3;
        }

        public static int GetMaxLevel(string availability)
        {
            var level = 0;
            var l = availability.Length;
            while (LevelOffset.GetLevelOffset(level) < l)
            {
                level++;
            }

            return level - 1;
        }

    }
}
