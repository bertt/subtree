# subtree

Reader/writer for Subtree binary format:

Specs: https://github.com/CesiumGS/3d-tiles/tree/draft-1.1/specification/ImplicitTiling#implicittiling-subtree-binary-format


## Tool subtree info:

Install:

```
$ dotnet tool install --global subtreeinfo
```

Or update:

```
$ dotnet tool update --global subtreeinfo
```

Sample:


```
$ subtreeinfo -i -i ./testfixtures/0.0.0.subtree

Subtree info
Action: Info
subtree file: ./testfixtures/0.0.0.subtree
Header magic: subt
Header version: 1
1] Tile availability:
Bitstream: 0
Available: 7
Availability: 101100000100110010000000
Number of levels: 3
Level: 0
1;

Level: 1
1-0;
0-1;

Level: 2
0-1-0-0;
1-0-0-0;
0-0-0-1;
0-0-1-0;

2] Content availability:
available: 0

3] Child subtree availability:
bitstream: 1
available: 8
Availability: 0000000000000000011000000000011001100000000001100000000000000000
0-0-1-0-0-0-0-0;
0-0-0-1-0-0-0-0;
1-0-0-0-0-0-0-0;
0-1-0-0-0-0-0-0;
0-0-0-0-0-0-1-0;
0-0-0-0-0-0-0-1;
0-0-0-0-1-0-0-0;
0-0-0-0-0-1-0-0;
```

## Samples code 

1] Writing 1 root tile

```
var subtree = new Subtree();
var t0 = BitArrayCreator.FromString("1");
subtree.TileAvailability = t0;

subtree.ContentAvailability = t0;

var bytes = SubtreeWriter.ToBytes(subtree);
File.WriteAllBytes("subtrees/0.0.0.subtree", bytes);
```

https://bertt.github.io/subtree/samples/roottile

## History

2022-09-02: Version 1.2 - add octree subdivision scheme support in subtreeInfo

2022-08-23: Version 1.1 - add tileAvailability

2022-07-20: Version 1.0.1

2022-07-20: Version 1.0

