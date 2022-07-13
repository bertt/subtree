// See https://aka.ms/new-console-template for more information
using Npgsql;
using quadtreewriter;
using subtree;
using Wkx;


var subtree_root = new Subtree();
var t0_root = BitArrayCreator.FromString("1");
subtree_root.TileAvailability = t0_root;

var s0_root = BitArrayCreator.FromString("1");
subtree_root.ContentAvailability= s0_root;

var bytes = SubtreeWriter.ToBytes(subtree_root);
File.WriteAllBytes("subtrees/0_0_0.subtree", bytes);


GenerateGlbFromDatabase();


static void GenerateGlbFromDatabase()
{
    Console.WriteLine("Hello, World!");
    var connectionString = "Host=::1;Username=postgres;Database=postgres;Port=5432;password=postgres";
    var conn = new NpgsqlConnection(connectionString);
    var bbox3d = GetBBox3D(conn);
    Console.WriteLine("bbox 3d: " + bbox3d);
    var translation = bbox3d.GetCenter().ToVector();
    var boundingboxAllFeatures = BoundingBoxCalculator.TranslateRotateX(bbox3d, Reverse(translation), Math.PI / 2);
    var box = boundingboxAllFeatures.GetBox();
    var sr = 4978;

    var geoms = GetGeometries(conn);

    Console.WriteLine("Geometries: " + geoms.Count);
    var triangles = GetTriangles(geoms);
    Console.WriteLine("Triangles: " + triangles.Count);
    var bytes = GlbCreator.GetGlb(triangles);
    File.WriteAllBytes("content/0_0_0.glb", bytes);
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

static List<Geometry> GetGeometries(NpgsqlConnection conn)
{
    var table = "delaware_buildings";
    var sql = $"SELECT ST_AsBinary(ST_RotateX(ST_Translate(geom_triangle, 1238070.0029833354 * -1, -4795867.907504121 * -1, 4006102.3617460253 * -1), -pi() / 2))FROM {table} where ST_GeometryType(geom_triangle) = 'ST_PolyhedralSurface'";
    conn.Open();
    var cmd = new NpgsqlCommand(sql, conn);
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