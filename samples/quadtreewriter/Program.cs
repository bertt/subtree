// See https://aka.ms/new-console-template for more information
using Npgsql;
using quadtreewriter;
using subtree;
using Wkx;

var tileAvailability = "11111";
var contentAvailability = "01111";
var subtreeAvailability = "1000000";

byte[] subtreebytes = GetSubtreeBytes(tileAvailability, contentAvailability);
File.WriteAllBytes("subtrees/0_0_0.subtree", subtreebytes);

var connectionString = "Host=::1;Username=postgres;Database=postgres;Port=5432;password=postgres";
var conn = new NpgsqlConnection(connectionString);
var bbox3d = GetBBox3D(conn);
var sr = 4978;

Console.WriteLine("bbox 3d: " + bbox3d);
var bbox = new BoundingBox(bbox3d.XMin, bbox3d.YMin, bbox3d.XMax, bbox3d.YMax);
for (var x = 0; x < 2; x++)
{
    for (var y = 0; y < 2; y++)
    {
        var dx = (bbox.XMax - bbox.XMin) / 2;
        var dy = (bbox.YMax - bbox.YMin) / 2;

        var xstart = bbox.XMin + dx * x;
        var ystart = bbox.YMin + dy * y;
        var xend = xstart + dx;
        var yend = ystart + dy;

        var bboxQuad = new BoundingBox(xstart, ystart, xend, yend);

        var bytes = GenerateGlbFromDatabase(conn, bboxQuad, sr);
        File.WriteAllBytes($"content/1_{x}_{y}.glb", bytes);

        //subtreebytes = GetSubtreeBytes(tileAvailability, contentAvailability);
        //File.WriteAllBytes($"subtrees/1_{x}_{y}.subtree", subtreebytes);
    }
}

static byte[] GenerateGlbFromDatabase(NpgsqlConnection conn, BoundingBox bbox, int epsg)
{
    Console.WriteLine("Hello, World!");
    //var translation = bbox3d.GetCenter().ToVector();
    //var boundingboxAllFeatures = BoundingBoxCalculator.TranslateRotateX(bbox3d, Reverse(translation), Math.PI / 2);
    //var box = boundingboxAllFeatures.GetBox();

    var geoms = GetGeometries(conn, bbox, epsg);

    Console.WriteLine("Geometries: " + geoms.Count);
    var triangles = GetTriangles(geoms);
    Console.WriteLine("Triangles: " + triangles.Count);
    var bytes = GlbCreator.GetGlb(triangles);

    // var b3dm = B3dmCreator.GetB3dm(triangles);
    // var bytes = b3dm.ToBytes();
    return bytes;
}

static List<quadtreewriter.Triangle> GetTriangles(List<Geometry> geoms)
{
    var triangleCollection = new List<quadtreewriter.Triangle>();
    foreach (var g in geoms)
    {
        var surface = (PolyhedralSurface)g;
        var triangles = Triangulator.GetTriangles(surface);

        triangleCollection.AddRange(triangles);
    }

    return triangleCollection;
}

static double[] Reverse(double[] translation)
{
    var res = new double[] { translation[0] * -1, translation[1] * -1, translation[2] * -1 };
    return res;
}


static BoundingBox3D GetBBox3D(NpgsqlConnection conn)
{
    conn.Open();
    var table = "delaware_buildings";
    var sql = $"SELECT st_xmin(geom1), st_ymin(geom1), st_zmin(geom1), st_xmax(geom1), st_ymax(geom1), st_zmax(geom1) FROM (select ST_3DExtent(geom_triangle) as geom1 from {table} where ST_GeometryType(geom_triangle) =  'ST_PolyhedralSurface' )  as t";
    var cmd = new NpgsqlCommand(sql, conn);
    var reader = cmd.ExecuteReader();
    reader.Read();
    var xmin = reader.GetDouble(0);
    var ymin = reader.GetDouble(1);
    var zmin = reader.GetDouble(2);
    var xmax = reader.GetDouble(3);
    var ymax = reader.GetDouble(4);
    var zmax = reader.GetDouble(5);
    reader.Close();
    conn.Close();
    return new BoundingBox3D() { XMin = xmin, YMin = ymin, ZMin = zmin, XMax = xmax, YMax = ymax, ZMax = zmax };
}

static List<Geometry> GetGeometries(NpgsqlConnection conn, BoundingBox bbox, int epsg)
{
    var table = "delaware_buildings";
    var sql = $"SELECT ST_AsBinary(ST_RotateX(ST_Translate(geom_triangle, 1238070.0029833354 * -1, -4795867.907504121 * -1, 4006102.3617460253 * -1), -pi() / 2))FROM {table}";
    var sqlWhere = $" WHERE ST_Intersects(ST_Centroid(ST_Envelope(geom_triangle)), ST_MakeEnvelope({bbox.XMin}, {bbox.YMin}, {bbox.XMax}, {bbox.YMax}, {epsg})) and ST_GeometryType(geom_triangle) = 'ST_PolyhedralSurface'";

    conn.Open();
    var cmd = new NpgsqlCommand(sql + sqlWhere, conn);
    var reader = cmd.ExecuteReader();

    var geometries = new List<Geometry>();
    while (reader.Read())
    {
        var stream = reader.GetStream(0);
        var geom = Geometry.Deserialize<WkbSerializer>(stream);
        geometries.Add(geom);
    }
    reader.Close();
    conn.Close();
    return geometries;
}

static byte[] GetSubtreeBytes(string tileAvailability, string contentAvailability, string subtreeAvailability=null)
{
    var subtree_root = new Subtree();
    var t0_root = BitArrayCreator.FromString(tileAvailability);
    subtree_root.TileAvailability = t0_root;

    var s0_root = BitArrayCreator.FromString(contentAvailability);
    subtree_root.ContentAvailability = s0_root;

    if (subtreeAvailability != null)
    {
        var c0_root = BitArrayCreator.FromString(subtreeAvailability);
        subtree_root.ChildSubtreeAvailability = c0_root;
    }

    var subtreebytes = SubtreeWriter.ToBytes(subtree_root);
    return subtreebytes;
}