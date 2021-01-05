using System.Linq;
using SnippetBuilder.Models;

namespace SnippetBuilder.Commands
{
    [Command(LongName = "name", ShortName = "n", Description = "Output file name")]
    public class NameCommand : CommandBase, IRecipeApplier
    {
        public override bool Validate()
        {
            return Params.Any();
        }

        public void ApplyTo(Recipe recipe)
        {
            recipe.Name = Parameter;
        }
    }
}