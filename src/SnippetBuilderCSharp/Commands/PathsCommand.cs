using System.IO;
using System.Linq;

namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "paths", ShortName = "p", Description = "Paths of target files or directories")]
    public class PathsCommand : CommandBase, IRecipeApplier
    {
        public override bool Validate()
        {
            return Params.Any() && Params.All(x => File.Exists(x) || Directory.Exists(x));
        }

        public void ApplyTo(Recipe recipe)
        {
            recipe.Paths = Params.ToArray();
        }
    }
}