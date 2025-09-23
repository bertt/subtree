namespace subtree;

public static class ContentToTileAvailability
{

    public static AvailabilityLevels3D GetTileAvailabilityLevels3D(AvailabilityLevels3D contentAvailabilitylevels)
    {
        var tileAvailabilitylevels3D = new AvailabilityLevels3D() { };

        var maxLevelNumber = contentAvailabilitylevels.Max(z => z.Level);
        for (var i = 0; i <= maxLevelNumber; i++)
        {
            tileAvailabilitylevels3D.Add(new AvailabilityLevel3D(i));
        }

        if (maxLevelNumber == 0 && contentAvailabilitylevels[0].BitArray3D.Get(0, 0, 0))
        {
            tileAvailabilitylevels3D[0].BitArray3D.Set(0, 0, 0, true);
        }

        for (var l = maxLevelNumber; l > 0; l--)
        {
            var currentContentLevel = contentAvailabilitylevels.Where(z => z.Level == l).FirstOrDefault();
            var currentTileLevel = tileAvailabilitylevels3D.Where(z => z.Level == l).FirstOrDefault();
            var parentTileLevel = tileAvailabilitylevels3D.Where(z => z.Level == l - 1).FirstOrDefault();
            var d = currentTileLevel.BitArray3D.GetDimension();

            for (var x = 0; x < d; x++)
            {
                for (var y = 0; y < d; y++)
                {
                    for(var z = 0; z < d; z++)
                    {
                        if (currentContentLevel.BitArray3D.Get(x, y, z) || currentTileLevel.BitArray3D.Get(x, y, z))
                        {
                            currentTileLevel?.BitArray3D.Set(x, y, z, true);
                            var parentX = x >> 1;
                            var parentY = y >> 1;
                            var parentZ = z >> 1;
                            parentTileLevel?.BitArray3D.Set(parentX, parentY, parentZ, true);
                        }
                    }
                }
            }
        }

        return tileAvailabilitylevels3D;
    }


    public static AvailabilityLevels GetTileAvailabilityLevels(AvailabilityLevels contentAvailabilitylevels)
    {
        var tileAvailabilitylevels = new AvailabilityLevels() { };
        var maxLevelNumber = contentAvailabilitylevels.Max(z => z.Level);
        for (var i = 0; i <= maxLevelNumber; i++)
        {
            tileAvailabilitylevels.Add(new AvailabilityLevel(i));
        }

        if (maxLevelNumber == 0 && contentAvailabilitylevels[0].BitArray2D.Get(0,0))
        {
            tileAvailabilitylevels[0].BitArray2D.Set(0, 0, true);
        }

        for (var l = maxLevelNumber; l > 0; l--)
        {
            var currentContentLevel = contentAvailabilitylevels.Where(z => z.Level == l).FirstOrDefault();
            var currentTileLevel = tileAvailabilitylevels.Where(z => z.Level == l).FirstOrDefault();
            var parentTileLevel = tileAvailabilitylevels.Where(z => z.Level == l - 1).FirstOrDefault();
            var w = currentTileLevel.BitArray2D.GetWidth();
            var h = currentTileLevel.BitArray2D.GetHeight();

            for (var x = 0; x < w; x++)
            {
                for (var y = 0; y < h; y++)
                {
                    if (currentContentLevel.BitArray2D.Get(x, y) || currentTileLevel.BitArray2D.Get(x, y))
                    {
                        currentTileLevel?.BitArray2D.Set(x, y, true);
                        var parentX = x >> 1;
                        var parentY = y >> 1;

                        parentTileLevel?.BitArray2D.Set(parentX, parentY, true);
                    }
                }
            }
        }

        return tileAvailabilitylevels;
    }

}
