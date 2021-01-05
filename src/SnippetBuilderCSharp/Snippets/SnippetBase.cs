using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using SnippetBuilderCSharp.Extensions;
using SnippetBuilderCSharp.IO;
using SnippetBuilderCSharp.Models;

namespace SnippetBuilderCSharp.Snippets
{
    public abstract class SnippetBase : ISnippet
    {
        protected abstract string Extension { get; }
        protected IFileBroker FileBroker { get; }
        protected IFileStreamBroker FileStreamBroker { get; }

        protected SnippetBase(IFileBroker fileBroker, IFileStreamBroker fileStreamBroker)
        {
            FileBroker = fileBroker;
            FileStreamBroker = fileStreamBroker;
        }

        public async ValueTask BuildAsync(Recipe recipe, CancellationToken cancellationToken = default)
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
                        input.AddRange(FileBroker.GetFilePaths(path, $"*"));
                    else
                        foreach (var extension in recipe.Extensions!)
                            input.AddRange(FileBroker.GetFilePaths(path, $"*{extension}"));
                }


            var outputDirectory = recipe.Output!;
            if (!FileBroker.ExistsDirectory(outputDirectory)) FileBroker.CreateDirectory(outputDirectory);

            var outputFile = Path.Combine(outputDirectory, recipe.Name! + Extension);
            var snippets = await BuildAsync(input, cancellationToken).ConfigureAwait(false);
            await FileStreamBroker.WriteLinesAsync(outputFile, snippets, cancellationToken)
                .ConfigureAwait(false);
        }

        public abstract ValueTask<IEnumerable<string>> BuildAsync(IEnumerable<string> paths,
            CancellationToken cancellationToken = default);
    }
}