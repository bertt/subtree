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

    public bool Available { get; set; }

    public int X { get; set; }
    public int Y { get; set; }
    public int Z { get; set; }

    public Tile3D Parent()
    {
        return new Tile3D(Level > 0 ? Level - 1 : Level, X >> 1, Y >> 1, Z >> 1);
    }

    public bool HasChild(Tile3D other)
    {
        if (Level >= other.Level) return false;
        var ld = other.Level - Level;

        var parent = other.Parent();
        for (var i = 0; i < ld - 1; ++i)
        {
            parent = parent.Parent();
        }

        return X == parent.X && Y == parent.Y && Z == parent.Z && Level == parent.Level;
    }

    public override bool Equals(object? obj)
    {
        if (obj is Tile3D other)
        {
            return Level == other.Level && X == other.X && Y == other.Y && Z == other.Z;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Level, X, Y, Z);
    }
}
