namespace subtree;

public static class SubtreeCreator
{
    public static byte[] GenerateSubtreefile(List<Tile> tiles)
    {
        var mortonIndices = MortonIndex.GetMortonIndices(tiles);
        var subtreebytes = SubtreeWriter.ToBytes(mortonIndices.tileAvailability, mortonIndices.contentAvailability);
        return subtreebytes;
    }

    public static Dictionary<Tile, byte[]> GenerateSubtreefiles(List<Tile> tiles)
    {
        var subtreeFiles = new Dictionary<Tile, byte[]>();
        var maxLevel = tiles.Max(s => s.Z);

        // generate child subtree files at halfway the levels
        // this formula could be adjusted for specific cases
        var subtreeLevel = (int)Math.Ceiling(((double)maxLevel + 1) / 2);

        if (subtreeLevel == 1)
        {
            var subtreeRoot = GenerateSubtreefile(tiles);
            subtreeFiles.Add(new Tile(0, 0, 0), subtreeRoot);
            return subtreeFiles;
        }

        var mortonIndices = MortonIndex.GetMortonIndices(tiles);
        var childSubtreeAvailabilty = Availability.GetLevelAvailability(mortonIndices.tileAvailability, subtreeLevel);

        var offset = LevelOffset.GetLevelOffset(subtreeLevel);
        var tileAvailability = mortonIndices.tileAvailability.Substring(0, offset);
        var contentAvailability = mortonIndices.contentAvailability.Substring(0, offset);

        var availabilityLength = tileAvailability.Length;

        // write the root subtree file
        var subtreeRootbytes = SubtreeWriter.ToBytes(tileAvailability, contentAvailability, childSubtreeAvailabilty);
        subtreeFiles.Add(new Tile(0, 0, 0), subtreeRootbytes);

        // now create the subtree files
        var ba = BitArray2DCreator.GetBitArray2D(childSubtreeAvailabilty);
        for (var x = 0; x < ba.GetWidth(); x++)
        {
            for (var y = 0; y < ba.GetHeight(); y++)
            {
                if (ba.Get(x, y))
                {
                    var t = new Tile(subtreeLevel, x, y);
                    var subtreeTiles = GetSubtreeTiles(tiles, t);
                    var mortonIndicesSubtree = MortonIndex.GetMortonIndices(subtreeTiles);
                    var subtreebytes = SubtreeWriter.ToBytes(Fill(mortonIndicesSubtree.tileAvailability, availabilityLength), Fill(mortonIndicesSubtree.contentAvailability, availabilityLength));
                    subtreeFiles.Add(t, subtreebytes);
                }
            }
        }

        return subtreeFiles;
    }

    public static string Fill(string availability, int targetLength)
    {
        var l = availability.Length;
        var res = availability + new string('0', targetLength - l);
        return res;
    }

    public static List<Tile> GetSubtreeTiles(List<Tile> tiles, Tile tile)
    {
        var res = new List<Tile>();
        var rootTile = new Tile(0, 0, 0);
        rootTile.Available = tiles.Where(x => x.Z == tile.Z && x.X == tile.X && x.Y == tile.Y).FirstOrDefault().Available;
        res.Add(rootTile);

        var children = tiles.Where(x => tile.HasChild(x));
        foreach (var child in children)
        {
            var rel = GetRelativeTile(tile, child);
            rel.Available = child.Available;
            res.Add(rel);
        }

        return res;
    }

    public static Tile GetRelativeTile(Tile from, Tile to)
    {
        var deltaZ = to.Z - from.Z;

        for (var i = 0; i < deltaZ; ++i)
        {
            from = new Tile(from.Z + 1, from.X * 2, from.Y * 2); ;
        }

        return new Tile(deltaZ, to.X - from.X, to.Y - from.Y);
    }
}