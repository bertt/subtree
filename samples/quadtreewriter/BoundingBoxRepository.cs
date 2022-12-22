using Npgsql;
using System.Globalization;
using Wkx;

namespace quadtreewriter;

public static class BoundingBoxRepository
{
    public static bool HasFeaturesInBox(NpgsqlConnection conn, string geometry_table, string geometry_column, Point from, Point to, int epsg, string lodQuery)
    {
        var fromX = from.X.Value.ToString(CultureInfo.InvariantCulture);
        var fromY = from.Y.Value.ToString(CultureInfo.InvariantCulture);
        var toX = to.X.Value.ToString(CultureInfo.InvariantCulture);
        var toY = to.Y.Value.ToString(CultureInfo.InvariantCulture);

        var sql = $"select exists(select {geometry_column} from {geometry_table} where ST_Intersects(ST_Centroid(ST_Envelope({geometry_column})), ST_MakeEnvelope({fromX}, {fromY}, {toX}, {toY}, {epsg})) and ST_GeometryType({geometry_column}) =  'ST_PolyhedralSurface' {lodQuery})";
        conn.Open();
        var cmd = new NpgsqlCommand(sql, conn);
        var reader = cmd.ExecuteReader();
        reader.Read();
        var exists = reader.GetBoolean(0);
        reader.Close();
        conn.Close();
        return exists;
    }

    public static int CountFeaturesInBox(NpgsqlConnection conn, string geometry_table, string geometry_column, Point from, Point to, int epsg, string lodQuery)
    {
        var fromX = from.X.Value.ToString(CultureInfo.InvariantCulture);
        var fromY = from.Y.Value.ToString(CultureInfo.InvariantCulture);
        var toX = to.X.Value.ToString(CultureInfo.InvariantCulture);
        var toY = to.Y.Value.ToString(CultureInfo.InvariantCulture);

        var sql = $"select count({geometry_column}) from {geometry_table} where ST_Intersects(ST_Centroid(ST_Envelope({geometry_column})), ST_MakeEnvelope({fromX}, {fromY}, {toX}, {toY}, {epsg})) and ST_GeometryType({geometry_column}) =  'ST_PolyhedralSurface' {lodQuery}";
        conn.Open();
        var cmd = new NpgsqlCommand(sql, conn);
        var reader = cmd.ExecuteReader();
        reader.Read();
        var count = reader.GetInt32(0);
        reader.Close();
        conn.Close();
        return count;
    }
}
