using subtree;
using System.Text;

WriteSample();

void WriteSample()
{
    var bytes = SubtreeWriter.ToBytes("1", "1");
    File.WriteAllBytes("subtrees/0.0.0.subtree", bytes);

    var fs = File.Create(@"subtrees/metadata.bin");

    var height = (double)1;
    var bytes1 = BitConverter.GetBytes(height);
    var paddedBytes = BufferPadding.AddPadding(bytes1);

    var writer = new BinaryWriter(fs);
    writer.Write(paddedBytes);
    fs.Close();
}
