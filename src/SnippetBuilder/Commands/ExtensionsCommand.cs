using SnippetBuilder.Models;

namespace SnippetBuilder.Commands
{
    [Command(ShortName = "e", LongName = "extensions", Description = "Includes file extensions (e.g.; .cs)")]
    public class ExtensionsCommand : CommandBase, IRecipeApplier
    {
        public void ApplyTo(Recipe recipe) => recipe.Extensions = Params.ToArray();

        public override bool Validate() => true;
    }
}