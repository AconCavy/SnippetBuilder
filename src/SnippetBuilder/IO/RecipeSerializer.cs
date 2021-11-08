using System.Text.Json;
using SnippetBuilder.Models;

namespace SnippetBuilder.IO;

internal class RecipeSerializer : IRecipeSerializer
{
    private readonly string[] _ext = { ".json" };

    private readonly IFileProvider _fileProvider;
    private readonly IFileStreamProvider _fileStreamBroker;

    public RecipeSerializer(IFileStreamProvider fileStreamProvider, IFileProvider fileProvider)
    {
        _fileStreamBroker = fileStreamProvider;
        _fileProvider = fileProvider;
    }

    public async IAsyncEnumerable<Recipe> DeserializeAsync(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            if (!_fileProvider.ExistsFile(path) && !_fileProvider.ExistsDirectory(path))
            {
                Console.WriteLine($"Skip ({path}), path does not exist.");
                continue;
            }

            if (!_ext.Contains(Path.GetExtension(path)))
            {
                Console.WriteLine($"Skip ({path}), path is invalid.");
                continue;
            }

            var lines = new List<string>();
            await foreach (var line in _fileStreamBroker.ReadLinesAsync(path))
            {
                lines.Add(line);
            }

            var json = string.Join("\n", lines);
            var recipes = JsonSerializer.Deserialize<Recipe[]>(json);
            if (recipes is null) continue;
            foreach (var recipe in recipes)
            {
                yield return recipe;
            }
        }
    }
}