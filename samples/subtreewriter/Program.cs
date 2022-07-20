using subtree;

Console.WriteLine("Hello, World!");
WriteLevel2Sample();

void WriteLevel2Sample()
{
    var subtree_root = new Subtree();
    var t0_root = BitArrayCreator.FromString("110010110000000000110000");
    subtree_root.TileAvailability = t0_root;
    subtree_root.ContentAvailability= BitArrayCreator.FromString("000000110000000000110000");

    var bytes = SubtreeWriter.ToBytes(subtree_root);
    File.WriteAllBytes("subtrees/0.0.0.subtree", bytes);
}

void WriteLevel1Sample()
{
    var subtree_root = new Subtree();
    var t0_root = BitArrayCreator.FromString("101100000100110010000000");
    subtree_root.TileAvailability = t0_root;
    var s0_root = BitArrayCreator.FromString("0000000000000000011000000000011001100000000001100000000000000000");

    subtree_root.ChildSubtreeAvailability= s0_root;

    var bytes = SubtreeWriter.ToBytes(subtree_root);
    File.WriteAllBytes("subtrees/0.0.0.subtree", bytes);

    var subtree = new Subtree();

    subtree.TileAvailability = BitArrayCreator.FromString("110010110000000000110000");
    subtree.ContentAvailability = BitArrayCreator.FromString("000000110000000000110000");
    var byteslevel1 = SubtreeWriter.ToBytes(subtree);
    File.WriteAllBytes($"subtrees/3.0.5.subtree", byteslevel1);
    File.WriteAllBytes($"subtrees/3.1.4.subtree", byteslevel1);
    File.WriteAllBytes($"subtrees/3.2.7.subtree", byteslevel1);
    File.WriteAllBytes($"subtrees/3.4.1.subtree", byteslevel1);
    File.WriteAllBytes($"subtrees/3.5.0.subtree", byteslevel1);
    File.WriteAllBytes($"subtrees/3.3.6.subtree", byteslevel1);
    File.WriteAllBytes($"subtrees/3.6.3.subtree", byteslevel1);
    File.WriteAllBytes($"subtrees/3.7.2.subtree", byteslevel1);
}
