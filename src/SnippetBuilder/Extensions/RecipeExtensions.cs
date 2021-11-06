using System.Linq;
using SnippetBuilder.Models;

namespace SnippetBuilder.Extensions;

public static class RecipeExtensions
{
    public static bool Validate(this Recipe recipe)
    {
        if (string.IsNullOrEmpty(recipe.Name)) return false;
        if (string.IsNullOrEmpty(recipe.Output)) return false;
        return recipe.Input is { } && !recipe.Input.Any(string.IsNullOrEmpty);
    }
}