using SnippetBuilder.Models;

namespace SnippetBuilder.Commands
{
    public interface IRecipeApplier
    {
        void ApplyTo(Recipe recipe);
    }
}