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

2022-12-22: Version 1.1.1: Adding method SubtreeWriter.ToBytes(string tileAvailability, string contentAvailability, string subtreeAvailability = null)

2022-08-23: Version 1.1 - add tileAvailability

2022-07-20: Version 1.0.1

2022-07-20: Version 1.0

