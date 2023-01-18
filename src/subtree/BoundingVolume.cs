
namespace subtree;
public class Boundingvolume
{
    private double[] _region;
    public double[] region
    {
        get
        {
            return _region;
        }
        set
        {
            _region = value.Select(d => Math.Round(d, 5)).ToArray();
        }
    }
}
