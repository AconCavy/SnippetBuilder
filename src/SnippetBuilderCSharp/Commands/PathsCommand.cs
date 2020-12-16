using System.Linq;
using SnippetBuilderCSharp.IO;

namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "paths", ShortName = "p", Description = "Paths of target files or directories")]
    public class PathsCommand : CommandBase, IRecipeApplier
    {
        private readonly IFileBroker _fileBroker;

        public PathsCommand(IFileBroker fileBroker)
        {
            _fileBroker = fileBroker;
        }

        public override bool Validate()
        {
            return Params.Any() && Params.All(x => _fileBroker.ExistsFile(x) || _fileBroker.ExistsDirectory(x));
        }

        public void ApplyTo(Recipe recipe)
        {
            recipe.Paths = Params.ToArray();
        }
    }
}