using System.IO;
using System.Linq;

namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "paths", ShortName = "p", Description = "Paths of target files or directories")]
    public class PathsCommand : CommandBase
    {
        public override bool Validate()
        {
            return Params.All(x => File.Exists(x) || Directory.Exists(x));
        }
    }
}