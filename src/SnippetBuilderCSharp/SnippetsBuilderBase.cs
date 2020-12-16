using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetBuilderCSharp
{
    public abstract class SnippetsBuilderBase : ISnippetsBuilder
    {
        protected List<string> FilePaths { get; }
        protected abstract string Extension { get; }
        private readonly string _outputDirectory;
        private readonly string _outputName;

        protected SnippetsBuilderBase(Recipe recipe)
        {
            if (recipe.Name is null) throw new ArgumentNullException(nameof(recipe.Name));
            if (recipe.Output is null) throw new ArgumentNullException(nameof(recipe.Output));
            if (recipe.Paths is null) throw new ArgumentNullException(nameof(recipe.Paths));

            _outputName = recipe.Name;
            FilePaths = new List<string>();
            foreach (var path in recipe.Paths)
                if (File.Exists(path)) FilePaths.Add(path);
                else if (Directory.Exists(path)) FilePaths.AddRange(Directory.GetFiles(path, "*.cs"));

            _outputDirectory = recipe.Output;
            if (!Directory.Exists(_outputDirectory)) Directory.CreateDirectory(_outputDirectory);
        }

        public async ValueTask BuildAsync(CancellationToken cancellationToken = default)
        {
            var outputPath = Path.Combine(_outputDirectory, _outputName + Extension);
            await using var fileStream = File.Exists(outputPath)
                ? new FileStream(outputPath, FileMode.Truncate)
                : File.Create(outputPath);
            await BuildSnippetsAsync(fileStream, cancellationToken);
            await fileStream.FlushAsync(cancellationToken);
        }

        protected abstract ValueTask BuildSnippetsAsync(FileStream fileStream, CancellationToken cancellationToken);
    }
}