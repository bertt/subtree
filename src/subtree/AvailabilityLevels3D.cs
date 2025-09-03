using System.Text;

namespace subtree;
public class AvailabilityLevels3D: List<AvailabilityLevel3D>
{
    public string ToMortonIndex()
    {
        var res = new StringBuilder();
        foreach (var level in this)
        {
            res.Append(level.ToMortonIndex());
        }
        return res.ToString();
    }

}
