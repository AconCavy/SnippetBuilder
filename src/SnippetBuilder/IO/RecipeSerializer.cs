using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SnippetBuilder.Models;

namespace SnippetBuilder.IO;

internal class RecipeSerializer : IRecipeSerializer
{
    private readonly string[] _ext = { ".json" };

    private readonly IFileBroker _fileBroker;
    private readonly IFileStreamBroker _fileStreamBroker;

    public RecipeSerializer(IFileStreamBroker fileStreamBroker, IFileBroker fileBroker)
    {
        _fileStreamBroker = fileStreamBroker;
        _fileBroker = fileBroker;
    }

    public async IAsyncEnumerable<Recipe> DeserializeAsync(IEnumerable<string> paths)
    {
        foreach (var path in paths)
        {
            if (!_fileBroker.ExistsFile(path) && !_fileBroker.ExistsDirectory(path))
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
            await foreach (var line in _fileStreamBroker.ReadLinesAsync(path)) lines.Add(line);

            var json = string.Join("\n", lines);
            var recipes = JsonSerializer.Deserialize<Recipe[]>(json);
            if (recipes is null) continue;
            foreach (var recipe in recipes) yield return recipe;
        }
    }
}