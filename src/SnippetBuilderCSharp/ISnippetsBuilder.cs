using System.Threading;
using System.Threading.Tasks;

namespace SnippetBuilderCSharp
{
    public interface ISnippetsBuilder
    {
        ValueTask BuildAsync(CancellationToken cancellationToken);
    }
}