using SnippetBuilderCSharp.Models;

namespace SnippetBuilderCSharp.Commands
{
    public interface IRecipeApplier
    {
        void ApplyTo(Recipe recipe);
    }
}