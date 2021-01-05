using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SnippetBuilderCSharp.IO;
using SnippetBuilderCSharp.Models;

namespace SnippetBuilderCSharp.Snippets
{
    public abstract class SnippetBase : ISnippet
    {
        protected List<string> FilePaths { get; }
        protected abstract string Extension { get; }
        protected IFileStreamBroker FileStreamBroker { get; }
        protected IFileBroker FileBroker { get; }

        private readonly string _outputDirectory;
        private readonly string _outputName;

        protected SnippetBase(Recipe recipe, IFileStreamBroker fileStreamBroker, IFileBroker fileBroker)
        {
            if (recipe.Name is null) throw new ArgumentNullException(nameof(recipe.Name));
            if (recipe.Output is null) throw new ArgumentNullException(nameof(recipe.Output));
            if (recipe.Paths is null) throw new ArgumentNullException(nameof(recipe.Paths));

            FileStreamBroker = fileStreamBroker;
            FileBroker = fileBroker;
            _outputName = recipe.Name;
            FilePaths = new List<string>();
            foreach (var path in recipe.Paths)
                if (FileBroker.ExistsFile(path)) FilePaths.Add(path);
                else if (FileBroker.ExistsDirectory(path)) FilePaths.AddRange(FileBroker.GetFilePaths(path, "*.cs"));

            _outputDirectory = recipe.Output;
            if (!FileBroker.ExistsDirectory(_outputDirectory)) FileBroker.CreateDirectory(_outputDirectory);
        }

        public async ValueTask BuildAsync(CancellationToken cancellationToken = default)
        {
            var outputPath = Path.Combine(_outputDirectory, _outputName + Extension);
            var snippets = await BuildSnippetsAsync(cancellationToken).ConfigureAwait(false);
            if (cancellationToken.IsCancellationRequested) return;
            await FileStreamBroker.WriteLinesAsync(outputPath, snippets, cancellationToken).ConfigureAwait(false);
        }

        public abstract ValueTask<IEnumerable<string>> BuildSnippetsAsync(
            CancellationToken cancellationToken = default);
    }
}