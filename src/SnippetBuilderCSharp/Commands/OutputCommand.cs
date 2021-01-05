using System.Linq;
using SnippetBuilderCSharp.Models;

namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "output", ShortName = "o", Description = "Output directory")]
    public class OutputCommand : CommandBase, IRecipeApplier
    {
        public override bool Validate()
        {
            return Params.Any();
        }

        public void ApplyTo(Recipe recipe)
        {
            recipe.Output = Parameter;
        }
    }
}