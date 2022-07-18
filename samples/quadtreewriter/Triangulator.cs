
using Wkx;

namespace quadtreewriter
{
    public static class Triangulator
    {
        public static List<Triangle> GetTriangles(PolyhedralSurface polyhedralsurface)
        {
            var degenerated_triangles = 0;
            var allTriangles = new List<Triangle>();
            for (var i = 0; i < polyhedralsurface.Geometries.Count; i++)
            {
                var geometry = polyhedralsurface.Geometries[i];
                var triangle = GetTriangle(geometry);

                if (triangle != null)
                {
                    allTriangles.Add(triangle);
                }
                else
                {
                    degenerated_triangles++;
                }
            }

            return allTriangles;
        }


        public static Triangle GetTriangle(Polygon geometry)
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
}
