using System.Collections;

namespace subtree;

public sealed class BitArray2D
{
    private BitArray _array;
    private int _dimension1;
    private int _dimension2;

    public BitArray2D(int dimension1, int dimension2)
    {
        _dimension1 = dimension1 > 0 ? dimension1 : throw new ArgumentOutOfRangeException(nameof(dimension1), dimension1, string.Empty);
        _dimension2 = dimension2 > 0 ? dimension2 : throw new ArgumentOutOfRangeException(nameof(dimension2), dimension2, string.Empty);
        _array = new BitArray(dimension1 * dimension2);
    }

    public bool Get(int x, int y) { CheckBounds(x, y); return _array[y * _dimension1 + x]; }
    public bool Set(int x, int y, bool val) { CheckBounds(x, y); return _array[y * _dimension1 + x] = val; }
    public bool this[int x, int y] { get { return Get(x, y); } set { Set(x, y, value); } }

    private void CheckBounds(int x, int y)
    {
        if (x < 0 || x >= _dimension1)
        {
            throw new IndexOutOfRangeException();
        }
        if (y < 0 || y >= _dimension2)
        {
            throw new IndexOutOfRangeException();
        }
    }

    public int GetWidth() { return _dimension1; }
    public int GetHeight() { return _dimension2; }

    public int GetDimension() { return _dimension1; }
    public bool IsAvailable()
    {
        return _array.OfType<bool>().Count(p => p) > 0;
    }

    public int Count(bool whereClause = false)
    {
        return _array.Count(whereClause);
    }

    public List<string> GetAvailableFiles(int rootZ = 0, int rootX = 0, int rootY = 0)
    {
        var level = Level.GetLevel(GetHeight() * GetWidth());
        var newLevel = rootZ + level;
        var baseX = rootX * 2 * level;
        var baseY = rootY * 2 * level;

        var availableFiles = new List<string>();

        for (var x = 0; x < GetWidth(); x++)
        {
            for (var y = 0; y < GetHeight(); y++)
            {
                if (Get(x, y))
                {
                    availableFiles.Add($"{newLevel}.{baseX + x}.{baseY + y}");
                }
            }
        }

        return availableFiles;
    }
}
