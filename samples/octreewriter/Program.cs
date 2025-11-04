using Npgsql;
using octreetreewriter;
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

if (!Directory.Exists("subtrees"))
{
    Directory.CreateDirectory("subtrees");
}

var maxFeaturesPerTile = 800;

Console.WriteLine("bbox 3d: " + bbox3d);

var bbox = new BoundingBox(bbox3d.XMin, bbox3d.YMin, bbox3d.XMax, bbox3d.YMax);
var center = new Point((bbox.XMin + bbox.XMax) / 2, (bbox.YMin + bbox.YMax) / 2, 0 );
var ecefCenter = GetEcef(conn, (double)center.X, (double)center.Y, (double)center.Z, epsg);

var rootTile3D = new Tile3D(0, 0, 0, 0);
var tiles3D = generateTiles3D(table, conn, epsg, geometry_column, ecefCenter, bbox3d, maxFeaturesPerTile, 0, rootTile3D, new List<Tile3D>());

// Generate subtree files using SubtreeCreator3D
var subtreeFiles = SubtreeCreator3D.GenerateSubtreefiles(tiles3D);
Console.WriteLine($"Generated {subtreeFiles.Count} subtree file(s)");

// Write all subtree files
foreach (var kvp in subtreeFiles)
{
    var tile = kvp.Key;
    var subtreeBytes = kvp.Value;
    var filename = $"subtrees/{tile.Level}_{tile.X}_{tile.Y}_{tile.Z}.subtree";
    File.WriteAllBytes(filename, subtreeBytes);
    Console.WriteLine($"Written subtree file: {filename}");
}
Console.WriteLine("Subtree files written, end of program");

var maxAvailableLevel = subtreeFiles.Max(p => p.Key.Level);
Console.WriteLine("Max available level: " + maxAvailableLevel);

Console.WriteLine($"In tileset.json use for subtreeLevels: {maxAvailableLevel}");

var transform = new float[] {
      1.0f,
      0.0f,
      0.0f,
      0.0f,
      0.0f,
      1.0f,
      0.0f,
      0.0f,
      0.0f,
      0.0f,
      1.0f,
      0.0f,
      ecefCenter.X,
      ecefCenter.Y,
      ecefCenter.Z,
      1.0f
};

var regionWgs84 = ToWgs84(conn, bbox3d, epsg);

var region = new float[]
            {
                (float)ToRadians(regionWgs84.XMin),
                (float)ToRadians(regionWgs84.YMin),
                (float)ToRadians(regionWgs84.XMax),
                (float)ToRadians(regionWgs84.YMax),
                (float)(regionWgs84.ZMin),
                (float)(regionWgs84.ZMax)
            };

var tileset = TilesetBuilder.CreateTilesetJson(transform,region,maxAvailableLevel);

File.WriteAllText("tileset.json", tileset);

Console.WriteLine("Program end");

static double ToRadians(double angle)
{
    return Math.PI * angle / 180.0;
}

static List<Tile3D> generateTiles3D(string table, NpgsqlConnection conn, int epsg, string geometryColumn, Vector3 translation, BoundingBox3D bbox, int maxFeaturesPerTile, int level, Tile3D tile, List<Tile3D> tiles)
{
    var numberOfFeatures = BoundingBoxRepository.CountFeaturesInBox(conn, table, geometryColumn, new Point(bbox.XMin, bbox.YMin, bbox.ZMin), new Point(bbox.XMax, bbox.YMax, bbox.ZMax), epsg);

    Console.WriteLine($"Features of tile {level}, {tile.Z},{tile.X},{tile.Y}: " + numberOfFeatures);

    if (numberOfFeatures == 0)
    {
        var t2 = new Tile3D(level, tile.X, tile.Y, tile.Z);
        t2.Available = false;
        tiles.Add(t2);
    }
    else if (numberOfFeatures > maxFeaturesPerTile)
    {
        Console.WriteLine($"Split tile in octree: {level},{tile.Z}_{tile.X}_{tile.Y} ");
        
        // Add the current tile as available but with no content (it's being subdivided)
        var currentTile = new Tile3D(level, tile.X, tile.Y, tile.Z);
        currentTile.Available = false; // No content at this level, subdivided
        tiles.Add(currentTile);
        
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

                    var new_tile = new Tile3D(level, tile.X * 2 + x, tile.Y * 2 + y, tile.Z * 2 + z);
                    generateTiles3D(table, conn, epsg, geometryColumn, translation, bbox3d, maxFeaturesPerTile, level, new_tile, tiles);
                }
            }
        }
    }
    else
    {
        Console.WriteLine($"Generate 3D tile: {tile.Level}, {tile.X}, {tile.Y}, {tile.Z}, ");
        var boundingBox = new BoundingBox(bbox.XMin, bbox.YMin, bbox.XMax, bbox.YMax);
        var bytes = GenerateGlbFromDatabase(conn, table, geometryColumn, translation, boundingBox, epsg, bbox.ZMin, bbox.ZMax);
        File.WriteAllBytes($"content/{tile.Level}_{tile.X}_{tile.Y}_{tile.Z}.glb", bytes);
        var t1 = new Tile3D(level, tile.X, tile.Y, tile.Z);
        t1.Available = true;
        tiles.Add(t1);
    }

    return tiles;
}

static byte[] GenerateGlbFromDatabase(NpgsqlConnection conn, string table, string geometryColumn, Vector3 translation, BoundingBox bbox, int epsg, double? zMin=null, double? zMax= null)
{
    var geoms = GetGeometries(conn, translation, table, geometryColumn, bbox, epsg, zMin, zMax);
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


static Vector3 GetEcef(NpgsqlConnection conn, double x, double y, double z, int sourceEpsg)
{
    conn.Open();
    var sql = $"select st_X(t.geom), st_Y(t.geom), st_Z(t.geom) from (SELECT st_transform(st_setsrid(ST_MakePoint({x.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {y.ToString(System.Globalization.CultureInfo.InvariantCulture)}, {z.ToString(System.Globalization.CultureInfo.InvariantCulture)}), {sourceEpsg}), 4978) as geom) as t";
    var cmd = new NpgsqlCommand(sql, conn);
    var reader = cmd.ExecuteReader(); 
    reader.Read();
    var xe = reader.GetDouble(0);
    var ye = reader.GetDouble(1);
    var ze = reader.GetDouble(2);
    reader.Close();
    conn.Close();
    return new Vector3((float)xe, (float)ye, (float)ze);
}

static BoundingBox3D ToWgs84(NpgsqlConnection conn, BoundingBox3D bbox, int sourceEpsg)
{
    conn.Open();
    var sql = $"SELECT st_xmin(geom1), st_ymin(geom1), st_xmax(geom1), st_ymax(geom1), st_zmin(geom1), st_zmax(geom1) FROM (select ST_3DExtent(st_transform(st_setsrid(ST_3DMakeBox(st_makepoint({bbox.XMin}, {bbox.YMin}, {bbox.ZMin}), st_makepoint({bbox.XMax}, {bbox.YMax}, {bbox.ZMax})), {sourceEpsg}), 4979)) as geom1)  as t";
    var cmd = new NpgsqlCommand(sql, conn);
    var reader = cmd.ExecuteReader();
    reader.Read();
    var xmin = reader.GetDouble(0);
    var ymin = reader.GetDouble(1);
        
    var xmax = reader.GetDouble(2);
    var ymax = reader.GetDouble(3);
    var zmin = reader.GetDouble(4);
    var zmax = reader.GetDouble(5);
    reader.Close();
    conn.Close();
    return new BoundingBox3D() { XMin = xmin, YMin = ymin, ZMin = zmin, XMax = xmax, YMax = ymax, ZMax = zmax };
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