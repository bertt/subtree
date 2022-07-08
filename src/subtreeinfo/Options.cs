using CommandLine;

namespace subtreeinfo
{
    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input path of the subtree file")]
        public string Input { get; set; }
    }
}
