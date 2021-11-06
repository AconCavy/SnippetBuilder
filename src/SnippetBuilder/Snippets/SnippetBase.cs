using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SnippetBuilder.Extensions;
using SnippetBuilder.IO;
using SnippetBuilder.Models;

namespace SnippetBuilder.Snippets;

public abstract class SnippetBase : ISnippet
{
    protected SnippetBase(IFileProvider fileProvider, IFileStreamProvider fileStreamProvider)
    {
        FileProvider = fileProvider;
        FileStreamProvider = fileStreamProvider;
    }

    protected abstract string Extension { get; }
    protected IFileProvider FileProvider { get; }
    protected IFileStreamProvider FileStreamProvider { get; }

    public async Task BuildAsync(Recipe recipe, CancellationToken cancellationToken = default)
    {
        if (!recipe.Validate()) throw new ArgumentException(nameof(recipe));

        var paths = new List<string>();
        foreach (var path in recipe.Input!)
        {
            if (FileProvider.ExistsFile(path))
            {
                paths.Add(path);
            }
            else if (FileProvider.ExistsDirectory(path))
            {
                if (recipe.Extensions is null || !recipe.Extensions.Any())
                {
                    paths.AddRange(FileProvider.GetFilePaths(path, "*"));
                }
                else
                {
                    foreach (var extension in recipe.Extensions!)
                    {
                        paths.AddRange(FileProvider.GetFilePaths(path, $"*{extension}"));
                    }
                }
            }
            else
            {
                Console.WriteLine($"Skip ({path}), path does not exist.");
            }
        }

        var outputDirectory = recipe.Output!;
        if (!FileProvider.ExistsDirectory(outputDirectory)) FileProvider.CreateDirectory(outputDirectory);

        var outputFile = Path.Combine(outputDirectory, recipe.Name! + Extension);
        var snippets = await BuildAsync(paths, cancellationToken).ConfigureAwait(false);
        await FileStreamProvider.WriteLinesAsync(outputFile, snippets, cancellationToken)
            .ConfigureAwait(false);
    }

    public abstract Task<IEnumerable<string>> BuildAsync(IEnumerable<string> paths,
        CancellationToken cancellationToken = default);
}