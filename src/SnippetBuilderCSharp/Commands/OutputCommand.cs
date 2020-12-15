using System.IO;
using System.Linq;

namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "output", ShortName = "o", Description = "Output directory")]
    public class OutputCommand : CommandBase
    {
        public override bool Validate()
        {
            return Params.All(x => File.Exists(x) || Directory.Exists(x));
        }
    }
}