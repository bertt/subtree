using CommandLine;
using Newtonsoft.Json;
using subtree;
using subtreeinfo;

Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
{
    Info(o);
});

void Info(Options options)
{
    Console.WriteLine($"Subtree info");
    Console.WriteLine($"Action: Info");
    Console.WriteLine("subtree file: " + options.Input);

    if (File.Exists(options.Input))
    {
        var subtreefile = File.OpenRead(options.Input);
        var subtree = SubtreeReader.ReadSubtree(subtreefile);
        Console.WriteLine("Header magic: " + subtree.SubtreeHeader.Magic);
        Console.WriteLine("Header version: " + subtree.SubtreeHeader.Version);

        var subtreeJsonObject = JsonConvert.DeserializeObject<SubtreeJson>(subtree.SubtreeJson);
        Console.WriteLine();
        Console.WriteLine("1] Tile availability: ");
        Console.WriteLine("Bitstream: " + subtreeJsonObject?.tileAvailability.bitstream);
        Console.WriteLine("Available: " + subtreeJsonObject?.tileAvailability.availableCount);

        var tileAvailability = subtree.TileAvailability.AsString();

        Console.WriteLine("Availability: " + tileAvailability);
        PrintAvailability(tileAvailability);

        Console.WriteLine("2] Content availability: ");
        if (subtreeJsonObject?.contentAvailability is not null)
        {
            foreach (var contentAvailability in subtreeJsonObject.contentAvailability)
            {
                if (contentAvailability.constant != 0)
                {
                    Console.WriteLine("bitstream: " + contentAvailability.bitstream);
                }
                Console.WriteLine("available: " + contentAvailability.availableCount);
            }
        }

        if (subtree.ContentAvailability != null)
        {
            Console.WriteLine("Availability: " + subtree.ContentAvailability.AsString());
            PrintAvailability(subtree.ContentAvailability.AsString(), true);
        }
        Console.WriteLine();
        Console.WriteLine("3] Child subtree availability: ");
        if (subtreeJsonObject?.childSubtreeAvailability.constant != 0)
        {
            Console.WriteLine("bitstream: " + subtreeJsonObject?.childSubtreeAvailability.bitstream);
        }

        Console.WriteLine("available: " + subtreeJsonObject?.childSubtreeAvailability.availableCount);
        Console.WriteLine();

        if (subtree.ChildSubtreeAvailability != null)
        {
            Console.WriteLine("Availability: " + subtree.ChildSubtreeAvailability.AsString());
            var childSubtreeAvailability = BitArray2DCreator.GetBitArray2D(subtree.ChildSubtreeAvailability.AsString());
            PrintBitArray2D(childSubtreeAvailability);
            Console.WriteLine("Subtree files expected: " + String.Join(',', subtree.GetExpectedSubtreeFiles()));
        }
    }
    else
    {
        Console.WriteLine($"Subtree file {options.Input} does not exist.");
    }
}


static void PrintAvailability(string availability, bool isContentAvailability = false)
{
    var l = LevelOffset.GetNumberOfLevels(availability, isContentAvailability);
    Console.WriteLine("Number of levels: " + l);

    for (int i = 0; i < l; i++)
    {
        Console.WriteLine("Level: " + i);
        var offset = LevelOffset.GetLevelOffset(i);
        var offset1 = LevelOffset.GetLevelOffset(i + 1);
        var levelAvailability = availability.Substring(offset, offset1 - offset);

        var availabilityArray = BitArray2DCreator.GetBitArray2D(levelAvailability);
        PrintBitArray2D(availabilityArray);
    }
}



static void PrintBitArray2D(BitArray2D bitArray2D)
{
    for (var y = bitArray2D.GetHeight() - 1; y >= 0; y--)
    {
        for (var x = 0; x < bitArray2D.GetWidth(); x++)
        {
            Console.Write(bitArray2D.Get(x, y) ? "1" : "0");
            Console.Write(x == bitArray2D.GetWidth() - 1 ? ";" : "-");
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}