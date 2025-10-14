using Npgsql;
using System.Globalization;
using Wkx;

namespace quadtreewriter;

public static class BoundingBoxRepository
{
    public static int CountFeaturesInBox(NpgsqlConnection conn, string geometry_table, string geometry_column, Point from, Point to, int epsg)
    {
        var hasZ = from.Z.HasValue && to.Z.HasValue;
        var fromX = from.X.Value.ToString(CultureInfo.InvariantCulture);
        var fromY = from.Y.Value.ToString(CultureInfo.InvariantCulture);
        var toX = to.X.Value.ToString(CultureInfo.InvariantCulture);
        var toY = to.Y.Value.ToString(CultureInfo.InvariantCulture);

        var sql = hasZ?
            $"select count({geometry_column}) from {geometry_table} where ST_3DIntersects(ST_Centroid(ST_Envelope({geometry_column})), ST_3DMakeBox(st_setsrid(ST_MakePoint({fromX}, {fromY}, {from.Z.Value.ToString(CultureInfo.InvariantCulture)}), {epsg}), st_setsrid(ST_MakePoint({toX}, {toY}, {to.Z.Value.ToString(CultureInfo.InvariantCulture)}), {epsg}))) ":
            $"select count({geometry_column}) from {geometry_table} where ST_Intersects(ST_Centroid(ST_Envelope({geometry_column})), ST_MakeEnvelope({fromX}, {fromY}, {toX}, {toY}, {epsg})) ";
        
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
