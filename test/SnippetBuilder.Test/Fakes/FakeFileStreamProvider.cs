using SnippetBuilder.IO;
using SnippetBuilder.Test.Utilities;

namespace SnippetBuilder.Test.Fakes;

public class FakeFileStreamProvider : IFileStreamProvider
{
    private readonly string[] _data =
    {
        "using System;", "", "public string Greet(string name)", "{", "    return $\"Hello {name}\"", "}"
    };

    public IAsyncEnumerable<string> ReadLinesAsync(string path) => _data.ToAsyncEnumerable();

    public async ValueTask<bool> WriteLinesAsync(string path, IEnumerable<string> data,
        CancellationToken cancellationToken = default) =>
        await Task.FromResult(true).ConfigureAwait(false);
}