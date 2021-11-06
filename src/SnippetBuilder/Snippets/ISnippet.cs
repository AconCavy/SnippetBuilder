using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SnippetBuilder.Models;

namespace SnippetBuilder.Snippets;

public interface ISnippet
{
    Task BuildAsync(Recipe recipe, CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> BuildAsync(IEnumerable<string> paths,
        CancellationToken cancellationToken = default);
}