namespace subtree;

public static class SubtreeCreator3D
{
    public static byte[] GenerateSubtreefile(List<Tile3D> tiles)
    {
        var mortonIndices = MortonIndex.GetMortonIndices3D(tiles);
        var subtreebytes = SubtreeWriter.ToBytes(mortonIndices.tileAvailability, mortonIndices.contentAvailability);
        return subtreebytes;
    }

    public static Dictionary<Tile, byte[]> GenerateSubtreefiles(List<Tile3D> tiles)
    {
        var subtreeFiles = new Dictionary<Tile, byte[]>();
        var maxLevel = tiles.Max(s => s.Level);

        // generate child subtree files at halfway the levels
        // this formula could be adjusted for specific cases
        var subtreeLevel = (int)Math.Ceiling(((double)maxLevel + 1) / 2);

        if (subtreeLevel == 1)
        {
            var subtreeRoot = GenerateSubtreefile(tiles);
            subtreeFiles.Add(new Tile(0, 0, 0), subtreeRoot);
            return subtreeFiles;
        }

        var mortonIndices = MortonIndex.GetMortonIndices3D(tiles);
        var childSubtreeAvailabilty = Availability.GetLevelAvailability(mortonIndices.tileAvailability, subtreeLevel, ImplicitSubdivisionScheme.Octree);

        var offset = LevelOffset.GetLevelOffset(subtreeLevel, ImplicitSubdivisionScheme.Octree);
        var tileAvailability = mortonIndices.tileAvailability.Substring(0, offset);
        var contentAvailability = mortonIndices.contentAvailability.Substring(0, offset);

        var availabilityLength = tileAvailability.Length;

        // write the root subtree file
        var subtreeRootbytes = SubtreeWriter.ToBytes(tileAvailability, contentAvailability, childSubtreeAvailabilty);
        subtreeFiles.Add(new Tile(0, 0, 0), subtreeRootbytes);

        // now create the subtree files
        var ba = BitArray3DCreator.GetBitArray3D(childSubtreeAvailabilty);
        for (var x = 0; x < ba.GetDimension(); x++)
        {
            for (var y = 0; y < ba.GetDimension(); y++)
            {
                for (var z = 0; z < ba.GetDimension(); z++)
                {
                    if (ba.Get(x, y, z))
                    {
                        var t = new Tile3D(subtreeLevel, x, y, z);
                        var subtreeTiles = GetSubtreeTiles(tiles, t);
                        var mortonIndicesSubtree = MortonIndex.GetMortonIndices3D(subtreeTiles);
                        var subtreebytes = SubtreeWriter.ToBytes(Fill(mortonIndicesSubtree.tileAvailability, availabilityLength), Fill(mortonIndicesSubtree.contentAvailability, availabilityLength));
                        // Convert Tile3D to Tile for dictionary key - encode z in the y coordinate offset
                        var tileKey = ConvertTile3DToTile(t);
                        subtreeFiles.Add(tileKey, subtreebytes);
                    }
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

    public static List<Tile3D> GetSubtreeTiles(List<Tile3D> tiles, Tile3D tile)
    {
        var res = new List<Tile3D>();
        var rootTile = new Tile3D(0, 0, 0, 0);
        var subtreeTile = tiles.Where(x => x.Level == tile.Level && x.X == tile.X && x.Y == tile.Y && x.Z == tile.Z).FirstOrDefault();
        rootTile.Available = subtreeTile?.Available ?? false;
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

    public static Tile3D GetRelativeTile(Tile3D from, Tile3D to)
    {
        var deltaLevel = to.Level - from.Level;

        for (var i = 0; i < deltaLevel; ++i)
        {
            from = new Tile3D(from.Level + 1, from.X * 2, from.Y * 2, from.Z * 2);
        }

        return new Tile3D(deltaLevel, to.X - from.X, to.Y - from.Y, to.Z - from.Z);
    }

    private static Tile ConvertTile3DToTile(Tile3D tile3D)
    {
        // For octree, we need to encode the 3D coordinates into a 2D Tile
        // We'll use a linearized index: linearIndex = x + y * width + z * width * width
        // where width = 2^level for the octree
        var width = 1 << tile3D.Level; // 2^level
        var linearIndex = tile3D.X + tile3D.Y * width + tile3D.Z * width * width;
        
        // Map to 2D: use linearIndex as x, and 0 as y
        return new Tile(tile3D.Level, linearIndex, 0);
    }
}
