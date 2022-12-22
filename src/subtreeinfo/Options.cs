using CommandLine;
using subtree;

namespace subtreeinfo;

public class Options
{
    [Option('i', "input", Required = true, HelpText = "Input path of the subtree file (Quadtree/Octree")]
    public string Input { get; set; }

    [Option('s', "SubdivisionScheme", Default = ImplicitSubdivisionScheme.Quadtree, Required = false, HelpText = "Input path of the subtree file")]
    public ImplicitSubdivisionScheme SubdivisonScheme{ get; set; }
}
