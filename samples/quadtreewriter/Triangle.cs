﻿using System.Numerics;
using Wkx;

namespace quadtreewriter;

public class Triangle
{
    private readonly Point p0, p1, p2;

    public Triangle(Point p0, Point p1, Point p2)
    {
        this.p0 = p0;
        this.p1 = p1;
        this.p2 = p2;
    }

    public Point GetP0()
    {
        return p0;
    }

    public Point GetP1()
    {
        return p1;
    }

    public Point GetP2()
    {
        return p2;
    }

    public Vector3 GetNormal()
    {
        var u = p2.Minus(p1);
        var vector_u = new Vector3(u.X, u.Y, u.Z);
        var v = p0.Minus(p1);
        var vector_v = new Vector3(v.X, v.Y, v.Z);
        var c = Vector3.Cross(vector_u, vector_v);
        var n = Vector3.Normalize(c);
        return n;
    }

    public bool IsDegenerated()
    {
        var v0 = new Vector3((float)p0.X, (float)p0.Y, (float)p0.Z);
        var v1 = new Vector3((float)p1.X, (float)p1.Y, (float)p1.Z);
        var v2 = new Vector3((float)p2.X, (float)p2.Y, (float)p2.Z);

        var isDegenerated = (v0.Equals(v1) || v1.Equals(v2)) || v2.Equals(v0);
        return isDegenerated;
    }

    public (Vector3, Vector3, Vector3) ToVectors()
    {
        var v0 = new Vector3((float)p0.X, (float)p0.Y, (float)p0.Z);
        var v1 = new Vector3((float)p1.X, (float)p1.Y, (float)p1.Z);
        var v2 = new Vector3((float)p2.X, (float)p2.Y, (float)p2.Z);
        return (v0, v1, v2);
    }
}
