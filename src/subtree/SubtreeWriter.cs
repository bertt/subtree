using Newtonsoft.Json;
using System.Collections;
using System.Text;

namespace subtree
{
    public static class SubtreeWriter
    {
        public static byte[] ToBytes(Subtree subtree)
        {
            var bin = ToSubtreeBinary(subtree);
            var subtreeJsonPadded = BufferPadding.AddPadding(JsonConvert.SerializeObject(bin.subtreeJson, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            }));
            var subtreeBinaryPadded = BufferPadding.AddPadding(bin.bytes);

            var memoryStream = new MemoryStream();
            var binaryWriter = new BinaryWriter(memoryStream);
            var subtreeHeader = new SubtreeHeader();
            subtreeHeader.JsonByteLength = (ulong)subtreeJsonPadded.Length;
            subtreeHeader.BinaryByteLength = (ulong)subtreeBinaryPadded.Length;

            binaryWriter.Write(subtreeHeader.AsBinary());
            binaryWriter.Write(Encoding.UTF8.GetBytes(subtreeJsonPadded));
            binaryWriter.Write(bin.bytes);

            //binaryWriter.Flush();
            binaryWriter.Close();
            var arr = memoryStream.ToArray();
            return arr;
        }

        public static (byte[] bytes, SubtreeJson subtreeJson) ToSubtreeBinary(Subtree subtree)
        {
            var substreamBinary = new List<byte>();
            var subtreeJson = new SubtreeJson();
            var bufferViews = new List<Bufferview>();
            var resultTileAvailability = HandleBitArrays(subtree.TileAvailability);
            bufferViews.Add(resultTileAvailability.bufferView);
            substreamBinary.AddRange(resultTileAvailability.bytes.ToArray());
            subtreeJson.tileAvailability = new Tileavailability() { bitstream = 0, availableCount = resultTileAvailability.trueBits };

            if (subtree.ContentAvailability != null)
            {
                var resultContentAvailability = HandleBitArrays(subtree.ContentAvailability);
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

            if (subtree.ChildSubtreeAvailability != null)
            {
                var resultSubstreamAvailability = HandleBitArrays(subtree.ChildSubtreeAvailability);
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

            subtreeJson.buffers = new List<Buffer>() { new Buffer() { byteLength = substreamBinary.Count } }.ToArray();
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
            bytes = BufferPadding.AddBinaryPadding(bytes.ToArray()).ToList();
            var bufferView = new Bufferview() { buffer = 0, byteLength = bitArrays.Count, byteOffset = 0 };
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
