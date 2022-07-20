namespace subtree
{
    public static class MortonIndex
    {
        public static string GetMortonIndex(List<Tile> tiles)
        {
            var availabilitylevels = new AvailabilityLevels();
            var maxZ = tiles.Max(t => t.Z);
            for (var z = 0; z <= maxZ; z++)
            {
                var levelTiles = tiles.Where(tile => tile.Z == z && tile.Available);
                var availabilityLevel = new AvailabilityLevel(z);
                foreach (var levelTile in levelTiles)
                {
                    availabilityLevel.BitArray2D.Set(levelTile.X, levelTile.Y, true);
                }
                availabilitylevels.Add(availabilityLevel);
            }
            var morton = availabilitylevels.ToMortonIndex();
            return morton;
        }


        public static byte[] GetMortonIndexAsBytes(List<Tile> tiles)
        {
            var mortonIndex = GetMortonIndex(tiles);
            var bit1 = BitArrayCreator.FromString(mortonIndex);
            var b = bit1.ToByteArray();
            return b;
        }
    }
}
