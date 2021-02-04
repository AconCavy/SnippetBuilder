using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SnippetBuilder.Extensions;
using SnippetBuilder.IO;
using SnippetBuilder.Models;

namespace SnippetBuilder.Snippets
{
    public abstract class SnippetBase : ISnippet
    {
        protected SnippetBase(IFileBroker fileBroker, IFileStreamBroker fileStreamBroker)
        {
            FileBroker = fileBroker;
            FileStreamBroker = fileStreamBroker;
        }

        protected abstract string Extension { get; }
        protected IFileBroker FileBroker { get; }
        protected IFileStreamBroker FileStreamBroker { get; }

        public async Task BuildAsync(Recipe recipe, CancellationToken cancellationToken = default)
        {
            if (!recipe.Validate()) throw new ArgumentException(nameof(recipe));

            var input = new List<string>();
            foreach (var path in recipe.Paths!)
                if (FileBroker.ExistsFile(path))
                {
                    input.Add(path);
                }
                else if (FileBroker.ExistsDirectory(path))
                {
                    if (recipe.Extensions is null || !recipe.Extensions.Any())
                        input.AddRange(FileBroker.GetFilePaths(path, "*"));
                    else
                        foreach (var extension in recipe.Extensions!)
                            input.AddRange(FileBroker.GetFilePaths(path, $"*{extension}"));
                }
                else
                {
                    Console.WriteLine($"Skip ({path}), path does not exist.");
                }

            var outputDirectory = recipe.Output!;
            if (!FileBroker.ExistsDirectory(outputDirectory)) FileBroker.CreateDirectory(outputDirectory);

            var outputFile = Path.Combine(outputDirectory, recipe.Name! + Extension);
            var snippets = await BuildAsync(input, cancellationToken).ConfigureAwait(false);
            await FileStreamBroker.WriteLinesAsync(outputFile, snippets, cancellationToken)
                .ConfigureAwait(false);
        }

        public abstract Task<IEnumerable<string>> BuildAsync(IEnumerable<string> paths,
            CancellationToken cancellationToken = default);
    }
}