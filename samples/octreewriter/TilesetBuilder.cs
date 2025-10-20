using System.Text.Json;

namespace octreetreewriter;
public static class TilesetBuilder
{
    public static string CreateTilesetJson(float[] transform, float[] region, int subtreeLevels)
    {
        var tileset = new Tileset.Rootobject
        {
            asset = new Tileset.Asset
            {
                generator = "",
                version = "1.1"
            },
            geometricError = 2000.0f,
            root = new Tileset.Root
            {
                transform = transform,
                geometricError = 2000.0f,
                refine = "ADD",
                boundingVolume = new Tileset.Boundingvolume
                {
                    region = region
                },
                content = new Tileset.Content
                {
                    uri = "content/{level}_{z}_{x}_{y}.glb"
                },
                implicitTiling = new Tileset.Implicittiling
                {
                    subdivisionScheme = "OCTREE",
                    subtreeLevels = subtreeLevels,
                    subtrees = new Tileset.Subtrees
                    {
                        uri = "subtrees/{level}_{z}_{x}_{y}.subtree"
                    }
                }
            }
        };

        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        return JsonSerializer.Serialize(tileset, options);
    }
}
