# subtree

Reader/writer for Subtree binary format:

Specs: https://github.com/CesiumGS/3d-tiles/tree/draft-1.1/specification/ImplicitTiling#implicittiling-subtree-binary-format

## Tool subtreeinfo

see https://github.com/bertt/subtree/blob/main/src/subtreeinfo/README.md

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

## History

2022-08-23: Version 1.1 - add tileAvailability

2022-07-20: Version 1.0.1

2022-07-20: Version 1.0

