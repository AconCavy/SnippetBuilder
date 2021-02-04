using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetBuilder.IO
{
    internal class FileStreamBroker : IFileStreamBroker
    {
        public async IAsyncEnumerable<string> ReadLinesAsync(string path)
        {
            await using var stream = new FileStream(path, FileMode.Open);
            using var streamReader = new StreamReader(stream);
            string? line;
            while ((line = await streamReader.ReadLineAsync()) != null) yield return line;
        }

        public async ValueTask<bool> WriteLinesAsync(string path, IEnumerable<string> data,
            CancellationToken cancellationToken = default)
        {
            await using var stream =
                File.Exists(path) ? new FileStream(path, FileMode.Truncate) : File.Create(path);
            await using var streamWriter = new StreamWriter(stream);
            foreach (var line in data)
            {
                await streamWriter.WriteLineAsync(line);
                if (cancellationToken.IsCancellationRequested) break;
            }

            await streamWriter.FlushAsync();
            return true;
        }
    }
}