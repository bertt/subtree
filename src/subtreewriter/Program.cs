using subtree;

Console.WriteLine("Hello, World!");
//WriteLevel1Sample();

WriteRootTileSample();

void WriteLevel1Sample()
{
    var subtree_root = new Subtree();
    var t0_root = BitArrayCreator.FromString("11111");
    subtree_root.TileAvailability = t0_root;

    var s0_root = BitArrayCreator.FromString("01111");
    subtree_root.ChildSubtreeAvailability= t0_root;

    var bytes = SubtreeWriter.ToBytes(subtree_root);
    File.WriteAllBytes("subtrees/0.0.0.subtree", bytes);


    for (var x = 0; x < 2; x++)
    {
        for (var y = 0; y < 2; y++)
        {
            var subtree = new Subtree();
            var t0 = BitArrayCreator.FromString("0");
            subtree.TileAvailability = t0;
            subtree.ContentAvailability = BitArrayCreator.FromString("1");

            var byteslevel1 = SubtreeWriter.ToBytes(subtree);
            File.WriteAllBytes($"subtrees/1.{x}.{y}.subtree", byteslevel1);
        }
    }
}



void WriteRootTileSample(){
    var subtree = new Subtree();
    var t0 = BitArrayCreator.FromString("1");
    subtree.TileAvailability = t0;

    var t1 = BitArrayCreator.FromString("1");
    subtree.ContentAvailability = t1;

    var bytes = SubtreeWriter.ToBytes(subtree);
    File.WriteAllBytes("subtrees/0.0.0.subtree", bytes);
}

