using System.Numerics;
using Wkx;

namespace quadtreewriter;

public static class PointExtensions
{
    public static Point Translate(this Point p, double dx, double dy, double dz)
    {
        return new Point((double)p.X + dx, (double)p.Y + dy, (double)p.Z + dz);
    }

    public static Point RotateX(this Point p, double angleRadians)
    {
        // formula from: https://cartosig.webs.upv.es/jaspa/v0.2.0/manual/html/ST_RotateX.html
        var x = p.X;
        var y = Math.Cos(angleRadians) * p.Y - Math.Sin(angleRadians) * p.Z;
        var z = Math.Sin(angleRadians) * p.Y + Math.Cos(angleRadians) * p.Z;
        return new Point((double)x, Math.Round((double)y, 5), Math.Round((double)z, 5));
    }

    public static double[] ToVector(this Point p)
    {
        var vector = new double[] { (double)p.X, (double)p.Y, (double)p.Z };

        return vector;
    }

    public static Vector3 Minus(this Point p, Point other)
    {
        var x = p.X - other.X;
        var y = p.Y - other.Y;
        var z = p.Z - other.Z;
        return new Vector3((float)x, (float)y, (float)z);
    }
}
