using subtree;

WriteSample();

void WriteSample()
{
    var bytes = SubtreeWriter.ToBytes("1", "1");
    File.WriteAllBytes("subtrees/0.0.0.subtree", bytes);
}
