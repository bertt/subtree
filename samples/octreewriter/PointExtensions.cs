using System.Numerics;
using Wkx;

namespace quadtreewriter;

public static class PointExtensions
{
    public static Vector3 Minus(this Point p, Point other)
    {
        var x = p.X - other.X;
        var y = p.Y - other.Y;
        var z = p.Z - other.Z;
        return new Vector3((float)x, (float)y, (float)z);
    }
}
