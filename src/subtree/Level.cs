namespace subtree;

public static class Level
{
    public static int GetLevel(int bitStreamLength, ImplicitSubdivisionScheme scheme = ImplicitSubdivisionScheme.Quadtree)
    {
        var power = scheme == ImplicitSubdivisionScheme.Quadtree? 4: 8;
        var level = (int)Math.Log(bitStreamLength, power);
        return Convert.ToInt32(level);
    }
}
