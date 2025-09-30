namespace subtree;

public static class MortonIndex
{

    public static (string tileAvailability, string contentAvailability) GetMortonIndices3D(List<Tile3D> tiles)
    {
        var contentAvailabilitylevels = new AvailabilityLevels3D();

        var maxLevel = tiles.Max(t => t.Level);
        for (var level = 0; level <= maxLevel; level++)
        {
            var levelTiles = tiles.Where(tile => tile.Level == level && tile.Available);
            var availabilityLevel = new AvailabilityLevel3D(level);
            foreach (var levelTile in levelTiles)
            {
                availabilityLevel.BitArray3D.Set(levelTile.X, levelTile.Y, levelTile.Z, true);
            }
            contentAvailabilitylevels.Add(availabilityLevel);
        }


        var tileAvailability = ContentToTileAvailability.GetTileAvailabilityLevels3D(contentAvailabilitylevels);
        var mortonContent = contentAvailabilitylevels.ToMortonIndex();
        return (tileAvailability.ToMortonIndex(), contentAvailabilitylevels.ToMortonIndex());
    }


    public static (string tileAvailability, string contentAvailability) GetMortonIndices(List<Tile> tiles)
    {
        var contentAvailabilitylevels = new AvailabilityLevels();

        var maxZ = tiles.Max(t => t.Z);
        for (var z = 0; z <= maxZ; z++)
        {
            var levelTiles = tiles.Where(tile => tile.Z == z && tile.Available);
            var availabilityLevel = new AvailabilityLevel(z);
            foreach (var levelTile in levelTiles)
            {
                availabilityLevel.BitArray2D.Set(levelTile.X, levelTile.Y, true);
            }
            contentAvailabilitylevels.Add(availabilityLevel);
        }


        var tileAvailability = ContentToTileAvailability.GetTileAvailabilityLevels(contentAvailabilitylevels);
        var mortonContent = contentAvailabilitylevels.ToMortonIndex();
        return (tileAvailability.ToMortonIndex(), contentAvailabilitylevels.ToMortonIndex());
    }


    public static (byte[] tileAvailability, byte[] contentAvailability) GetMortonIndexAsBytes(List<Tile> tiles)
    {
        var mortonIndices = GetMortonIndices(tiles);
        var bitsTileAvailability = BitArrayCreator.FromString(mortonIndices.tileAvailability).ToByteArray();
        var bitsContentAvailability = BitArrayCreator.FromString(mortonIndices.contentAvailability).ToByteArray();
        return (bitsTileAvailability, bitsContentAvailability);
    }


    public static (byte[] tileAvailability, byte[] contentAvailability) GetMortonIndexAsBytes3D(List<Tile3D> tiles)
    {
        var mortonIndices = GetMortonIndices3D(tiles);
        var bitsTileAvailability = BitArrayCreator.FromString(mortonIndices.tileAvailability).ToByteArray();
        var bitsContentAvailability = BitArrayCreator.FromString(mortonIndices.contentAvailability).ToByteArray();
        return (bitsTileAvailability, bitsContentAvailability);
    }

}
