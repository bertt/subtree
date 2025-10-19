
## Tool subtree info:

Parameters:

```
Usage: subtreeinfo [options]
Options:
  -i, --input <file>         Input subtree file path
  -s, --subdivision <scheme> Subdivision scheme: Quadtree (default) or Octree
  -h, --help                 Show help information
```

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
$ subtreeinfo -i ./testfixtures/0.0.0.subtree

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

## History

2024-10-01: version 1.2.0: From .NET 6 to .NET 8

2022-09-02: Version 1.1 - add octree subdivision scheme support (parameter s, default Quadtree)

2022-07-11: Version 1.0
