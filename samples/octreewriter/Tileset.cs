namespace octreetreewriter;

public class Tileset
{

    public class Rootobject
    {
        public Asset asset { get; set; }
        public float geometricError { get; set; }
        public Root root { get; set; }
    }

    public class Asset
    {
        public string generator { get; set; }
        public string version { get; set; }
    }

    public class Root
    {
        public float[] transform { get; set; }
        public float geometricError { get; set; }
        public string refine { get; set; }
        public Boundingvolume boundingVolume { get; set; }
        public Content content { get; set; }
        public Implicittiling implicitTiling { get; set; }
    }

    public class Boundingvolume
    {
        public float[] region { get; set; }
    }

    public class Content
    {
        public string uri { get; set; }
    }

    public class Implicittiling
    {
        public string subdivisionScheme { get; set; }
        public int subtreeLevels { get; set; }
        public Subtrees subtrees { get; set; }
    }

    public class Subtrees
    {
        public string uri { get; set; }
    }

}
