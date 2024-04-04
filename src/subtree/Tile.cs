namespace subtree;
public class Tile
{
    public Tile(int z, int x, int y)
    {
        Z = z;
        X = x;
        Y = y;
    }

    public Tile(int z, int x, int y, bool available) : this(z, x, y)
    {
        Available = available;
    }

    public string ContentUri { get; set; }
    public int Lod { get; set; }

    public int Z { get; set; }
    public int X { get; set; }
    public int Y { get; set; }

    public double ZMin { get; set; }

    public double ZMax { get; set; }
    public List<Tile> GetChildren()
    {
        var t1 = new Tile(X * 2, Y * 2, Z + 1);
        var t2 = new Tile(X * 2 + 1, Y * 2, Z + 1);
        var t3 = new Tile(X * 2 + 1, Y * 2 + 1, Z + 1);
        var t4 = new Tile(X * 2, Y * 2 + 1, Z + 1);
        return new List<Tile>() { t1, t2, t3, t4 };
    }
    public bool Available { get; set; }
    public double GeometricError { get; set; }

    public List<Tile> Children { get; set; }

    public double[] BoundingBox { get; set; }

    public Tile Parent()
    {
        return new Tile(Z > 0 ? Z - 1 : Z, X >> 1, Y >> 1);
    }

    public bool HasChild(Tile other)
    {
        if (Z >= other.Z) return false;
        var ld = other.Z - Z;

        var parent = other.Parent();
        for (var i = 0; i < ld - 1; ++i)
        {
            parent = parent.Parent();
        }

        return X == parent.X && Y == parent.Y && Z == parent.Z;
    }
}
