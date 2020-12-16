using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetBuilderCSharp.IO
{
    public interface IFileStreamBroker
    {
        IAsyncEnumerable<string> ReadLinesAsync(string path);

        ValueTask<bool> WriteLinesAsync(string path, IEnumerable<string> data,
            CancellationToken cancellationToken = default);
    }
}