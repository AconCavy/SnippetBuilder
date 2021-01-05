﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SnippetBuilderCSharp.Models;

namespace SnippetBuilderCSharp.Snippets
{
    public interface ISnippet
    {
        ValueTask BuildAsync(Recipe recipe, CancellationToken cancellationToken = default);

        ValueTask<IEnumerable<string>> BuildAsync(IEnumerable<string> paths,
            CancellationToken cancellationToken = default);
    }
}