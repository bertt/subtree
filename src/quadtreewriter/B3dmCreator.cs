namespace quadtreewriter
{
    public static class B3dmCreator
    {
        public static B3dm.Tile.B3dm GetB3dm(List<Triangle> triangleCollection)
        {
            var bytes = GlbCreator.GetGlb(triangleCollection);
            var b3dm = new B3dm.Tile.B3dm(bytes);

            return b3dm;
        }
    }
}
