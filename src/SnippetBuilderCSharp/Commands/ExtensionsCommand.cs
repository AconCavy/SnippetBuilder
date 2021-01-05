using SnippetBuilderCSharp.Models;

namespace SnippetBuilderCSharp.Commands
{
    [Command(ShortName = "e", LongName = "extensions", Description = "Includes file extensions (e.g.; .cs)")]
    public class ExtensionsCommand : CommandBase, IRecipeApplier
    {
        public override bool Validate()
        {
            return true;
        }

        public void ApplyTo(Recipe recipe)
        {
            recipe.Extensions = Params.ToArray();
        }
    }
}