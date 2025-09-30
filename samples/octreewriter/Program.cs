// See https://aka.ms/new-console-template for more information
using subtree;

Console.WriteLine("Hello, World!");

var tile3d = new Tile3D(0, 0, 0, 0);
tile3d.Available = false;

var tiles = new List<Tile3D>() { tile3d };


for(var x = 0 ; x < 2; x++)
    for(var y = 0; y < 2; y++)
        for(var z = 0; z < 2; z++)
        {
            var t = new Tile3D(1, x, y, z);
            t.Available = true;
            tiles.Add(t);
        }

var mortonIndices = MortonIndex.GetMortonIndices3D(tiles);

Console.WriteLine("Morton index: " + mortonIndices.contentAvailability);

Console.WriteLine("Morton index: " + mortonIndices.tileAvailability);


