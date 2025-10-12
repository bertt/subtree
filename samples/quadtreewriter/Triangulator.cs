
using Wkx;

namespace quadtreewriter;

public static class Triangulator
{
    public static List<Triangle> GetTriangles(Tin tin)
    {
        var allTriangles = new List<Triangle>();
        for (var i = 0; i < tin.Geometries.Count; i++)
        {
            var geometry = tin.Geometries[i];
            var triangle = GetTriangle(geometry);

            if (triangle != null)
            {
                allTriangles.Add(triangle);
            }
        }

        return allTriangles;
    }


    public static Triangle? GetTriangle(Polygon geometry)
    {
        var triangle = ToTriangle(geometry);

        if (!triangle.IsDegenerated())
        {
            return triangle;
        }
        return null;
    }

    private static Triangle ToTriangle(Polygon geometry)
    {
        var pnts = geometry.ExteriorRing.Points;
        if (pnts.Count != 4)
        {
            throw new ArgumentOutOfRangeException($"Expected number of vertices in triangles: 4, actual: {pnts.Count}");
        }

        var triangle = new Triangle(pnts[0], pnts[1], pnts[2]);
        return triangle;
    }
}
