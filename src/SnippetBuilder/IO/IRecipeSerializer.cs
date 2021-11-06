using System.Collections.Generic;
using SnippetBuilder.Models;

namespace SnippetBuilder.IO;

public interface IRecipeSerializer
{
    public IAsyncEnumerable<Recipe> DeserializeAsync(IEnumerable<string> paths);
}