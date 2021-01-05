using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SnippetBuilder.IO;
using SnippetBuilder.Models;

namespace SnippetBuilder.Commands
{
    [Command(LongName = "recipe", ShortName = "r", Description = "Recipe file path (json)")]
    public class RecipeCommand : CommandBase
    {
        private readonly string[] _ext = {".json"};
        private readonly IFileStreamBroker _fileStreamBroker;
        private readonly IFileBroker _fileBroker;

        public RecipeCommand(IFileStreamBroker fileStreamBroker, IFileBroker fileBroker)
        {
            _fileStreamBroker = fileStreamBroker;
            _fileBroker = fileBroker;
        }

        public override bool Validate()
        {
            return Params.Any() && Params.All(x => _fileBroker.ExistsFile(x) && _ext.Contains(Path.GetExtension(x)));
        }

        public async IAsyncEnumerable<Recipe> GetRecipesAsync()
        {
            foreach (var path in Params)
            {
                var lines = new List<string>();
                await foreach (var line in _fileStreamBroker.ReadLinesAsync(path)) lines.Add(line);

                var json = string.Join("\n", lines);
                var recipes = JsonSerializer.Deserialize<Recipe[]>(json);
                if (recipes is null) continue;
                foreach (var recipe in recipes) yield return recipe;
            }
        }
    }
}