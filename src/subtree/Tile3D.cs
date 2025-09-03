namespace subtree;
public class Tile3D
{
    public Tile3D(int level, int x, int y, int z)
    {
        Level = level;
        X = x;
        Y = y;
        Z = z;
    }

    public int Level { get; set; }
    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }
}
