using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetBuilderCSharp
{
    public abstract class BaseSnippetsBuilder : ISnippetsBuilder
    {
        protected List<string> FilePaths { get; }
        protected abstract string Extension { get; }
        private readonly string _outputDirectory;
        private readonly string _outputName;

        protected BaseSnippetsBuilder(IEnumerable<string> targets, string outputDirectory, string outputName)
        {
            _outputName = outputName;
            FilePaths = new List<string>();
            foreach (var target in targets)
                if (File.Exists(target)) FilePaths.Add(target);
                else if (Directory.Exists(target)) FilePaths.AddRange(Directory.GetFiles(target, "*.cs"));

            _outputDirectory = outputDirectory;
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