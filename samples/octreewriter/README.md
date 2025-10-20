### Octreewriter

Demo writing 3D Tiles with impliciti tiling in Octree format from PostGIS.

Sample data see ./data/kieivitsweg.gpkg

GDAL import sample:

```
ogr2ogr -f "PostgreSQL" ^
  PG:"host=localhost port=5439 dbname=postgres user=postgres password=postgres" ^
  "kievitsweg.gpkg" ^
  -nln "ifc.v_kievitsweg" ^
  -nlt PROMOTE_TO_MULTI ^
  -dim 3 ^
  -lco GEOMETRY_NAME=geometry ^
  -lco SPATIAL_INDEX=YES ^
  -overwrite
```

GDAL export sample:

```
ogr2ogr -f "GPKG" "kievitsweg.gpkg" ^
  PG:"host=localhost port=5439 dbname=postgres user=postgres password=postgres" ^
  -sql "SELECT * FROM ifc.v_kievitsweg" ^
  -nln "v_kievitsweg" ^
  -nlt PROMOTE_TO_MULTI ^
  -dim 3 ^
  -lco GEOMETRY_NAME=geometry ^
  -lco SPATIAL_INDEX=YES
```