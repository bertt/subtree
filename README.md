# subtree

Reader/writer for Subtree binary format:

Specs: https://github.com/CesiumGS/3d-tiles/tree/draft-1.1/specification/ImplicitTiling#implicittiling-subtree-binary-format

NuGet: https://www.nuget.org/packages/subtree

## Tool subtreeinfo

see https://github.com/bertt/subtree/blob/main/src/subtreeinfo/README.md

NuGet: https://www.nuget.org/packages/subtreeinfo

## Samples code 

1] Writing 1 root tile

```
var bytes = SubtreeWriter.ToBytes("1","1");
File.WriteAllBytes("subtrees/0.0.0.subtree", bytes);
```

## History

2023-04-09: version 1.4.7: no rounding of bounding volume box

2023-04-04: Version 1.4.5, 1.4.6: Add ZMin and ZMax as Tile properties

2023-02-21: Version 1.4.3, 1.4.4: Change error handling

2023-02-20: Version 1.4.1, 1.4.2: Add error handling

2023-02-16: Version 1.4: Add implicit tiling subtree file functionality

2023-01-17: Version 1.3.1: Removing dependency wkx 

2023-01-17: Version 1.3: Adding suport for multiple subtree files

2022-12-22: Version 1.2: Adding method SubtreeWriter.ToBytes(string tileAvailability, string contentAvailability, string subtreeAvailability = null)

2022-08-23: Version 1.1 - add tileAvailability

2022-07-20: Version 1.0.1

2022-07-20: Version 1.0

