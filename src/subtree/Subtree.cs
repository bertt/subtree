using Newtonsoft.Json;
using System.Collections;
using System.Text;

namespace subtree
{
    public class Subtree
    {
        public SubtreeHeader SubtreeHeader { get; set; } = null!;
        public string SubtreeJson { get; set; } = null!;
        public byte[] SubtreeBinary { get; set; } = null!;

        public List<BitArray>? ChildSubtreeAvailability { get; set; }
        public List<BitArray> TileAvailability { get; set; } = null!;
        public List<BitArray>? ContentAvailability { get; set; }

        public byte[] ToBytes()
        {
            var bin = ToSubtreeBinary();
            var subtreeJsonPadded = BufferPadding.AddPadding(JsonConvert.SerializeObject(bin.subtreeJson, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            var subtreeBinaryPadded = BufferPadding.AddPadding(SubtreeBinary);

            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            SubtreeHeader.JsonByteLength  = (ulong)subtreeJsonPadded.Length;
            SubtreeHeader.BinaryByteLength = (ulong)subtreeBinaryPadded.Length;

            binaryWriter.Write(SubtreeHeader.AsBinary());
            binaryWriter.Write(Encoding.UTF8.GetBytes(subtreeJsonPadded));
            binaryWriter.Write(bin.bytes);

            binaryWriter.Flush();
            binaryWriter.Close();
            return memoryStream.ToArray();
        }


        public (byte[] bytes, SubtreeJson subtreeJson) ToSubtreeBinary()
        {
            var substreamBinary = new List<Byte>();
            var subtreeJson = new SubtreeJson();
            var bufferViews = new List<Bufferview>();
            var resultTileAvailability = HandleBitArrays(TileAvailability);
            bufferViews.Add(resultTileAvailability.bufferView);
            substreamBinary.AddRange(resultTileAvailability.bytes.ToArray());
            subtreeJson.tileAvailability = new Tileavailability() { bitstream = 0, availableCount = resultTileAvailability.trueBits };

            if (ContentAvailability != null)
            {
                var resultContentAvailability = HandleBitArrays(ContentAvailability);
                var bufferView = resultContentAvailability.bufferView;
                bufferView.byteOffset = substreamBinary.Count;
                subtreeJson.childSubtreeAvailability = new Childsubtreeavailability() { bitstream = bufferViews.Count, availableCount = resultContentAvailability.trueBits };
                bufferViews.Add(bufferView);
                substreamBinary.AddRange(resultContentAvailability.bytes.ToArray());
            }
            else
            {
                subtreeJson.contentAvailability = new List<Contentavailability>() { new Contentavailability() { availableCount = 0, constant = 0 } }.ToArray();
            }

            if (ChildSubtreeAvailability != null)
            {
                var resultSubstreamAvailability = HandleBitArrays(ChildSubtreeAvailability);
                var bufferView = resultSubstreamAvailability.bufferView;
                bufferView.byteOffset = substreamBinary.Count;
                subtreeJson.childSubtreeAvailability = new Childsubtreeavailability() { bitstream = bufferViews.Count, availableCount = resultSubstreamAvailability.trueBits };
                bufferViews.Add(bufferView);
                substreamBinary.AddRange(resultSubstreamAvailability.bytes.ToArray());
            }
            else
            {
                // todo
            }

            subtreeJson.buffers = new List<Buffer>() { new Buffer() { byteLength = substreamBinary.Count} }.ToArray();
            subtreeJson.bufferViews = bufferViews.ToArray();
            return (substreamBinary.ToArray(), subtreeJson);
        }

        private static (List<byte> bytes, int trueBits, Bufferview bufferView) HandleBitArrays(List<BitArray> bitArrays)
        {
            var bytes = new List<byte>();
            var trueBits = 0;
            foreach (var bitArray in bitArrays)
            {
                var res = HandleBitArray(bitArray);
                trueBits += res.trueBits;
                bytes.AddRange(res.bytes);
            }
            var bufferView = new Bufferview() { buffer = 0, byteLength = bytes.Count, byteOffset = 0 };
            bytes = BufferPadding.AddBinaryPadding(bytes.ToArray()).ToList();
            return (bytes, trueBits, bufferView);
        }


        private static (List<byte> bytes, int trueBits, Bufferview bufferView) HandleBitArray(BitArray bitArray)
        {
            var bytes = new List<byte>();
            var trueBits = 0;
            trueBits += bitArray.Count(true);
            bytes.Add(bitArray.ToByte());
            var bufferView = new Bufferview() { buffer = 0, byteLength = bytes.Count, byteOffset = 0 };
            return (bytes, trueBits, bufferView);
        }
    }
}
