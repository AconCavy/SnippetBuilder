using System.Linq;
using SnippetBuilder.Models;

namespace SnippetBuilder.Extensions
{
    public static class RecipeExtensions
    {
        public static bool Validate(this Recipe recipe)
        {
            if (string.IsNullOrEmpty(recipe.Name)) return false;
            if (string.IsNullOrEmpty(recipe.Output)) return false;
            if (recipe.Paths is null || recipe.Paths.Any(string.IsNullOrEmpty)) return false;
            return true;
        }
    }
}