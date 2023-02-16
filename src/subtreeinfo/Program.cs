using CommandLine;
using Newtonsoft.Json;
using subtree;
using subtreeinfo;

Parser.Default.ParseArguments<Options>(args).WithParsed(o =>
{
    var t = new Tile(0, 0, 0);
    Info(o);
});

void Info(Options options)
{
    Console.WriteLine($"Subtree info");
    Console.WriteLine($"Action: Info");
    Console.WriteLine("subtree file: " + options.Input);

    if (File.Exists(options.Input))
    {
        var scheme = options.SubdivisonScheme;
        Console.WriteLine($"Subdivision scheme: {scheme}");

        var subtreefile = File.OpenRead(options.Input);
        var subtree = SubtreeReader.ReadSubtree(subtreefile);
        Console.WriteLine("Header magic: " + subtree.SubtreeHeader.Magic);
        Console.WriteLine("Header version: " + subtree.SubtreeHeader.Version);

        var subtreeJsonObject = JsonConvert.DeserializeObject<SubtreeJson>(subtree.SubtreeJson);
        Console.WriteLine();
        Console.WriteLine("1] Tile availability: ");
        Console.WriteLine("Bitstream: " + subtreeJsonObject?.tileAvailability.bitstream);
        Console.WriteLine("Available: " + subtreeJsonObject?.tileAvailability.availableCount);
        Console.WriteLine("Constant: " + subtreeJsonObject?.tileAvailability.constant);

        if (subtree.TileAvailability != null)
        {
            Console.WriteLine("Length: " + subtree.TileAvailability.Length + " bits");
            var tileAvailability = subtree.TileAvailability.AsString();
            //Console.WriteLine($"TileAvailability: {tileAvailability}"); 
            PrintAvailability(tileAvailability,  scheme);
        }
        Console.WriteLine("2] Content availability: ");
        if (subtreeJsonObject?.contentAvailability is not null)
        {
            foreach (var contentAvailability in subtreeJsonObject.contentAvailability)
            {
                Console.WriteLine("Bitstream: " + contentAvailability.bitstream);
                Console.WriteLine("Constant: " + contentAvailability.constant);
                Console.WriteLine("Available: " + contentAvailability.availableCount);
            }
        }

        if (subtree.ContentAvailability != null)
        {
            //Console.WriteLine($"ContentAvailability: {subtree.ContentAvailability.AsString()}");
            Console.WriteLine("Length: " + subtree.ContentAvailability.Length + " bits");
            var content = subtree.ContentAvailability.AsString();
            PrintAvailability(content, scheme);

            var files = new List<string>();
            var level = LevelOffset.GetNumberOfLevels(content, scheme);
            for(var l = 0; l < level; l++)
            {
                var ba = Availability.GetLevel(content, l);
                // to get absolute tiles when using multiple subtree change following call:
                files.AddRange(ba.GetAvailableFiles(0, 0, 0));
            }
            Console.WriteLine($"Tiles expected (first 3 of {files.Count}): " + String.Join(',', files.Take(3)));
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
            var level = Level.GetLevel(subtree.ChildSubtreeAvailability.Length);
            Console.WriteLine($"Level for subtree files: {level}");
            var childSubtreeAvailability = BitArray2DCreator.GetBitArray2D(subtree.ChildSubtreeAvailability.AsString());
            PrintBitArray2D(childSubtreeAvailability);
            var files = childSubtreeAvailability.GetAvailableFiles();
            Console.WriteLine($"Subtree files expected (first 3 of {files.Count}): " + String.Join(',', files.Take(3)));
        }
    }
    else
    {
        Console.WriteLine($"Subtree file {options.Input} does not exist.");
    }
}

static void PrintAvailability(string availability, ImplicitSubdivisionScheme scheme=ImplicitSubdivisionScheme.Quadtree)
{

    var l = LevelOffset.GetNumberOfLevels(availability, scheme);
    Console.WriteLine("Number of levels: " + l);

    var total = 0;

    for (int i = 0; i < l; i++)
    {
        var ba = Availability.GetLevel(availability, i);
        var levelAvailable = ba.Count(true);
        var levelTotal = ba.GetWidth() * ba.GetHeight();
        total += levelTotal;
        
        Console.WriteLine($"Level: {i}, available {levelAvailable}/{levelTotal}");
    }
    Console.WriteLine($"Total: {total}");


    if(scheme == ImplicitSubdivisionScheme.Quadtree)
    {
        var maxLevel = l;
        if (l > 4)
        {
            maxLevel = 4;
            Console.WriteLine($"Printing level [0..{maxLevel-1}] of {l}...");
        }
        else
        {
            Console.WriteLine($"Printing level [0..{maxLevel-1}]...");
        }
        Console.WriteLine("");
        for (var j = 0; j < maxLevel; j++)
        {
            var offset = LevelOffset.GetLevelOffset(j);
            var offset1 = LevelOffset.GetLevelOffset(j + 1);
            var levelAvailability = availability.Substring(offset, offset1 - offset);
            var availabilityArray = BitArray2DCreator.GetBitArray2D(levelAvailability);
            PrintBitArray2D(availabilityArray);
        }
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