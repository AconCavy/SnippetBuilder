using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace SnippetBuilderCSharp.Commands
{
    [Command(LongName = "recipe", ShortName = "r", Description = "Recipe file path (json)")]
    public class RecipeCommand : CommandBase
    {
        private readonly string[] _ext = {".json"};

        public override bool Validate()
        {
            return Params.Any() && Params.All(x => File.Exists(x) && _ext.Contains(Path.GetExtension(x)));
        }

        public async IAsyncEnumerable<Recipe> GetRecipesAsync()
        {
            foreach (var path in Params)
            {
                await using var stream = new FileStream(path, FileMode.Open);
                var recipes = await JsonSerializer.DeserializeAsync<Recipe[]>(stream);
                if (recipes is null) continue;
                foreach (var recipe in recipes) yield return recipe;
            }
        }
    }
}