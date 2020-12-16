using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetBuilderCSharp
{
    public interface ISnippetsBuilder
    {
        ValueTask BuildAsync(CancellationToken cancellationToken);
        ValueTask<IEnumerable<string>> BuildSnippetsAsync(CancellationToken cancellationToken = default);
    }
}