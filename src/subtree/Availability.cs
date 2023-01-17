namespace subtree;
public static class Availability
{
    public static BitArray2D? GetLevel(string availability, int Level, ImplicitSubdivisionScheme scheme = ImplicitSubdivisionScheme.Quadtree)
    {
        var offset = LevelOffset.GetLevelOffset(Level, scheme);
        var offset1 = LevelOffset.GetLevelOffset(Level + 1, scheme);
        var levelAvailability = availability.Substring(offset, offset1 - offset);
        var ba = BitArray2DCreator.GetBitArray2D(levelAvailability);
        return ba;
    }

}
