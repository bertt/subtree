using SharpGLTF.Geometry;
using SharpGLTF.Geometry.VertexTypes;
using SharpGLTF.Materials;
using SharpGLTF.Scenes;
using System.Drawing;
using System.Numerics;

namespace quadtreewriter;


public static class GlbCreator
{
    public static byte[]? GetGlb(List<Triangle> triangles, string copyright = "")
    {
        var color = "#D94F33"; // "#bb3333";
        var rgb = ColorTranslator.FromHtml(color);

        var material = new MaterialBuilder().
        WithDoubleSide(true).
        WithMetallicRoughnessShader().
        WithAlpha(AlphaMode.BLEND).
        WithChannelParam(KnownChannel.BaseColor, ColorToVector4(rgb));

        var mesh = new MeshBuilder<VertexPositionNormal, VertexWithBatchId, VertexEmpty>("mesh");

        foreach (var triangle in triangles)
        {
            DrawTriangle(triangle, material, mesh);
        }
        var scene = new SceneBuilder();
        scene.AddRigidMesh(mesh, Matrix4x4.Identity);
        var model = scene.ToGltf2();
        model.Asset.Copyright = copyright;
        var bytes = model.WriteGLB().Array;

        return bytes;
    }

    private static bool DrawTriangle(Triangle triangle, MaterialBuilder material, MeshBuilder<VertexPositionNormal, VertexWithBatchId, VertexEmpty> mesh)
    {
        var normal = triangle.GetNormal();
        var prim = mesh.UsePrimitive(material);
        var vectors = triangle.ToVectors();
        var indices = prim.AddTriangleWithBatchId(vectors, normal, 0);
        return indices.Item1 > 0;
    }


    private static Vector4 ColorToVector4(Color c)
    {
        var v = new Vector4((float)c.R / 255, (float)c.G / 255, (float)c.B / 255, (float)c.A / 255);
        return v;
    }

}
