using System.Linq;
using SnippetBuilder.Models;

namespace SnippetBuilder.Commands
{
    [Command(LongName = "output", ShortName = "o", Description = "Output directory")]
    public class OutputCommand : CommandBase, IRecipeApplier
    {
        public void ApplyTo(Recipe recipe) => recipe.Output = Parameter;

        public override bool Validate() => Params.Any();
    }
}