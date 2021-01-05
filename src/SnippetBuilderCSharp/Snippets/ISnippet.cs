using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SnippetBuilderCSharp.Snippets
{
    public interface ISnippet
    {
        ValueTask BuildAsync(CancellationToken cancellationToken);
        ValueTask<IEnumerable<string>> BuildSnippetsAsync(CancellationToken cancellationToken = default);
    }
}