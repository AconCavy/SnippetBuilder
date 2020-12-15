using System.IO;
using System.Linq;

namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "recipe", ShortName = "r", Description = "Recipe file path (json)")]
    public class RecipeCommand : CommandBase
    {
        private readonly string[] _ext = {".json"};

        public override bool Validate()
        {
            return Params.Any() && Params.All(x => File.Exists(x) && _ext.Contains(Path.GetExtension(x)));
        }
    }
}