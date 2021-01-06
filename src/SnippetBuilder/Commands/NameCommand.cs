using System.Linq;
using SnippetBuilder.Models;

namespace SnippetBuilder.Commands
{
    [Command(LongName = "name", ShortName = "n", Description = "Output file name")]
    public class NameCommand : CommandBase, IRecipeApplier
    {
        public void ApplyTo(Recipe recipe) => recipe.Name = Parameter;

        public override bool Validate() => Params.Any();
    }
}