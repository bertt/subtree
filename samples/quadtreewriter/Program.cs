// See https://aka.ms/new-console-template for more information
using Npgsql;
using quadtreewriter;
using subtree;
using System.Diagnostics;
using Wkx;

var table = "delaware_buildings";
var stopwatch = new Stopwatch();
stopwatch.Start();

var connectionString = "Host=::1;Username=postgres;Database=postgres;Port=5432;password=postgres";
var conn = new NpgsqlConnection(connectionString);
var bbox3d = GetBBox3D(conn);
var epsg = 4978;
var geometry_column = "geom_triangle";

Console.WriteLine("bbox 3d: " + bbox3d);
var bbox = new BoundingBox(bbox3d.XMin, bbox3d.YMin, bbox3d.XMax, bbox3d.YMax);

var maxFeaturesPerTile = 1000;

var tile = new Tile(0, 0, 0);
var tiles = generateTiles(table, conn, epsg, geometry_column, bbox, maxFeaturesPerTile, tile, new List<Tile>());
Console.WriteLine("tiles:" + tiles.Count);

stopwatch.Stop();
Console.WriteLine(stopwatch.ElapsedMilliseconds / 1000 + "s");
var maxAvailableLevel = tiles.Max(x => x.Z);
Console.WriteLine("Max available level: " + maxAvailableLevel);

// sample: 00000001000010001011000001110000011101001000001010000111100101111000011010000000000000000000000000000000000000000111100000000000000000000000000001111000000001111000000000000000000001111000011110000000000000000000000000000000000001111111100001111000000000000000000000000000000000000000011110000000000000000000000000000000000000000000000000000
string morton = MortonIndex.GetMortonIndex(tiles);
Console.WriteLine("Morton index: " + morton);

var subtreebytes = GetSubtreeBytes(morton);
File.WriteAllBytes($"subtrees/0_0_0.subtree", subtreebytes);

var s = 0;


static List<Tile> generateTiles(string table, NpgsqlConnection conn, int epsg, string geometry_column, BoundingBox bbox, int maxFeaturesPerTile, Tile tile, List<Tile> tiles)
{
    var numberOfFeatures = BoundingBoxRepository.CountFeaturesInBox(conn, table, geometry_column, new Point(bbox.XMin, bbox.YMin), new Point(bbox.XMax, bbox.YMax), epsg, "");

    Console.WriteLine($"Features of tile {tile.Z},{tile.X},{tile.Y}: " + numberOfFeatures);
    if (numberOfFeatures == 0)
    {
        var t2 = new Tile(tile.Z, tile.X, tile.Y);
        t2.Available = false;
        tiles.Add(t2);
    }
    else if (numberOfFeatures > maxFeaturesPerTile)
    {
        Console.WriteLine($"Split tile in quads: {tile.Z}_{tile.X}_{tile.Y} ");
        // split in quadtree
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

                var new_tile = new Tile(tile.Z + 1, tile.X * 2 + x, tile.Y * 2 + y);
                generateTiles(table, conn, epsg, geometry_column, bboxQuad, maxFeaturesPerTile, new_tile, tiles);
            }
        }
    }
    else
    {
        Console.WriteLine($"Generate tile: {tile.Z}, {tile.X}, {tile.Y}");
        var bytes = GenerateGlbFromDatabase(conn, table, bbox, epsg);
        File.WriteAllBytes($"content/test_{tile.Z}_{tile.X}_{tile.Y}.glb", bytes);
        var t1 = new Tile(tile.Z, tile.X, tile.Y);
        t1.Available = true;
        tiles.Add(t1);
    }

    return tiles;
}

static byte[] GenerateGlbFromDatabase(NpgsqlConnection conn, string table, BoundingBox bbox, int epsg)
{
    var geoms = GetGeometries(conn, table, bbox, epsg);
    var triangles = GetTriangles(geoms);
    var bytes = GlbCreator.GetGlb(triangles);
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

static List<Geometry> GetGeometries(NpgsqlConnection conn, string table, BoundingBox bbox, int epsg)
{
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

static byte[] GetSubtreeBytes(string contentAvailability, string subtreeAvailability = null)
{
    var subtree_root = new Subtree();
    // var t0_root = BitArrayCreator.FromString(tileAvailability);
    subtree_root.TileAvailabiltyConstant = 1;

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
