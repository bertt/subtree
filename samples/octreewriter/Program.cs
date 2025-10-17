using Npgsql;
using quadtreewriter;
using subtree;
using System.Diagnostics;
using System.Numerics;
using Wkx;

var table = "ifc.kievitsweg";
var geometry_column = "geometry";
var stopwatch = new Stopwatch();
stopwatch.Start();

var connectionString = "Host=::1;Username=postgres;Database=postgres;Port=5439;password=postgres";
var conn = new NpgsqlConnection(connectionString);
var bbox3d = GetBBox3D(conn, table, geometry_column);
var epsg = 7415;

if (!Directory.Exists("content"))
{
    Directory.CreateDirectory("content");
}

var maxFeaturesPerTile = 800;

Console.WriteLine("bbox 3d: " + bbox3d);

var bbox = new BoundingBox(bbox3d.XMin, bbox3d.YMin, bbox3d.XMax, bbox3d.YMax);

var rootTile3D = new Tile3D(0, 0, 0, 0);
var tiles3D = generateTiles3D(table, conn, epsg, geometry_column, bbox3d, maxFeaturesPerTile, 0, rootTile3D, new List<Tile3D>());
var mortonIndices = MortonIndex.GetMortonIndices3D(tiles3D);
Console.WriteLine("Morton index: " + mortonIndices.contentAvailability);

var subtreebytes = GetSubtreeBytes(mortonIndices.contentAvailability, mortonIndices.tileAvailability);
File.WriteAllBytes($"subtrees/0_0_0_0.subtree", subtreebytes);
Console.WriteLine("Subtree file is written, en d of program");

var maxAvailableLevel = tiles3D.Max(p => p.Level);
Console.WriteLine("Max available level: " + maxAvailableLevel);

Console.WriteLine($"In tileset.json use for subtreeLevels: {maxAvailableLevel + 1}");
Console.WriteLine("Program end");

static List<Tile> generateTiles(string table, NpgsqlConnection conn, int epsg, string geometryColumn, BoundingBox bbox, int maxFeaturesPerTile, Tile tile, List<Tile> tiles)
{
    var numberOfFeatures = BoundingBoxRepository.CountFeaturesInBox(conn, table, geometryColumn, new Point(bbox.XMin, bbox.YMin), new Point(bbox.XMax, bbox.YMax), epsg);

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
                generateTiles(table, conn, epsg, geometryColumn, bboxQuad, maxFeaturesPerTile, new_tile, tiles);
            }
        }
    }
    else
    {
        Console.WriteLine($"Generate tile: {tile.Z}, {tile.X}, {tile.Y}");
        var bytes = GenerateGlbFromDatabase(conn, table, geometryColumn, bbox, epsg);
        File.WriteAllBytes($"content/{tile.Z}_{tile.X}_{tile.Y}.glb", bytes);
        var t1 = new Tile(tile.Z, tile.X, tile.Y);
        t1.Available = true;
        tiles.Add(t1);
    }

    return tiles;
}



static List<Tile3D> generateTiles3D(string table, NpgsqlConnection conn, int epsg, string geometryColumn, BoundingBox3D bbox, int maxFeaturesPerTile, int level, Tile3D tile, List<Tile3D> tiles)
{
    var numberOfFeatures = BoundingBoxRepository.CountFeaturesInBox(conn, table, geometryColumn, new Point(bbox.XMin, bbox.YMin, bbox.ZMin), new Point(bbox.XMax, bbox.YMax, bbox.ZMax), epsg);

    Console.WriteLine($"Features of tile {level}, {tile.Z},{tile.X},{tile.Y}: " + numberOfFeatures);

    if(level == 1 && tile.X == 1 && tile.Y == 0 && tile.Z ==0)
    {
        Console.WriteLine("debug");
    }
    if (numberOfFeatures == 0)
    {
        var t2 = new Tile3D(level, tile.Z, tile.X, tile.Y);
        t2.Available = false;
        tiles.Add(t2);
    }
    else if (numberOfFeatures > maxFeaturesPerTile)
    {
        Console.WriteLine($"Split tile in octree: {level},{tile.Z}_{tile.X}_{tile.Y} ");
        level++;
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


                for (var z = 0; z < 2; z++)
                {
                    var dz = (bbox.ZMax - bbox.ZMin) / 2;
                    var z_start = bbox.ZMin + dz * z;
                    var zend = z_start + dz;
                    var bbox3d = new BoundingBox3D(xstart, ystart, z_start, xend, yend, zend);

                    var new_tile = new Tile3D(level, tile.X * 2 + x, tile.Y * 2 + y, tile.Z + z);
                    generateTiles3D(table, conn, epsg, geometryColumn, bbox3d, maxFeaturesPerTile, level, new_tile, tiles);
                }
            }
        }
    }
    else
    {
        Console.WriteLine($"Generate 3D tile: {tile.Level}, {tile.X}, {tile.Y}, {tile.Z}, ");
        var boundingBox = new BoundingBox(bbox.XMin, bbox.YMin, bbox.XMax, bbox.YMax);
        var bytes = GenerateGlbFromDatabase(conn, table, geometryColumn, boundingBox, epsg, bbox.ZMin, bbox.ZMax);
        File.WriteAllBytes($"content/{tile.Level}_{tile.Z}_{tile.X}_{tile.Y}.glb", bytes);
        var t1 = new Tile3D(level, tile.X, tile.Y, tile.Z);
        t1.Available = true;
        tiles.Add(t1);
    }

    return tiles;
}

static byte[] GenerateGlbFromDatabase(NpgsqlConnection conn, string table, string geometryColumn, BoundingBox bbox, int epsg, double? zMin=null, double? zMax= null)
{
    var v3 = new Vector3(3932794.25f, 316347.9375f, 4994553.5f);
    var geoms = GetGeometries(conn, v3, table, geometryColumn, bbox, epsg, zMin, zMax);
    var triangles = GetTriangles(geoms);
    var bytes = GlbCreator.GetGlb(triangles);
    return bytes;
}

static List<quadtreewriter.Triangle> GetTriangles(List<Geometry> geoms)
{
    var triangleCollection = new List<quadtreewriter.Triangle>();
    foreach (var g in geoms)
    {
        var surface = (Tin)g;
        var triangles = Triangulator.GetTriangles(surface);

        triangleCollection.AddRange(triangles);
    }

    return triangleCollection;
}

static BoundingBox3D GetBBox3D(NpgsqlConnection conn, string tableName, string geometry)
{
    conn.Open();
    var sql = $"SELECT st_xmin(geom1), st_ymin(geom1), st_zmin(geom1), st_xmax(geom1), st_ymax(geom1), st_zmax(geom1) FROM (select ST_3DExtent({geometry}) as geom1 from {tableName})  as t";
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

static List<Geometry> GetGeometries(NpgsqlConnection conn, Vector3 translation, string table, string geometryColumn, BoundingBox bbox, int epsg, double? zMin=null, double? zMax = null)
{
    var hasZ = zMin.HasValue && zMax.HasValue;  
    var sql = $"SELECT ST_AsBinary(ST_Translate(st_transform({geometryColumn},4978), {translation.X} * -1, {translation.Y} * -1, {translation.Z} * -1)) FROM {table}";

    var sqlWhere = hasZ?
        $" WHERE ST_3DIntersects(ST_Centroid(ST_Envelope({geometryColumn})), ST_3DMakeBox(st_setsrid(ST_MakePoint({bbox.XMin}, {bbox.YMin}, {zMin.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}), {epsg}), st_setsrid(ST_MakePoint({bbox.XMax}, {bbox.YMax}, {zMax.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}), {epsg}))) " :
        $" WHERE ST_Intersects(ST_Centroid(ST_Envelope({geometryColumn})), ST_MakeEnvelope({bbox.XMin}, {bbox.YMin}, {bbox.XMax}, {bbox.YMax}, {epsg}))";

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

static byte[] GetSubtreeBytes(string contentAvailability, string tileAvailability, string subtreeAvailability = null)
{
    var subtree_root = new Subtree();
    var s01_root = BitArrayCreator.FromString(tileAvailability);
    subtree_root.TileAvailability = s01_root;

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