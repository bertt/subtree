namespace subtree;
public static class Availability
{
    public static string GetLevelAvailability(string availability, int Level, ImplicitSubdivisionScheme scheme = ImplicitSubdivisionScheme.Quadtree)
    {
        var offset = LevelOffset.GetLevelOffset(Level, scheme);
        var offset1 = LevelOffset.GetLevelOffset(Level + 1, scheme);
        var levelAvailability = availability.Substring(offset, offset1 - offset);
        return levelAvailability;
    }

    public static BitArray2D? GetLevel(string availability, int Level, ImplicitSubdivisionScheme scheme = ImplicitSubdivisionScheme.Quadtree)
    {
        var levelAvailability = GetLevelAvailability(availability, Level, scheme);
        var ba = BitArray2DCreator.GetBitArray2D(levelAvailability);
        return ba;
    }
}
