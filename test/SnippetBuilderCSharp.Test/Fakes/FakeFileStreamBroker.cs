﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SnippetBuilderCSharp.IO;

namespace SnippetBuilderCSharp.Test.Fakes
{
    public class FakeFileStreamBroker : IFileStreamBroker
    {
        private readonly string[] _data =
        {
            "using System;",
            "",
            "public string Greet(string name)",
            "{",
            "    return $\"Hello {name}\"",
            "}"
        };

        public async IAsyncEnumerable<string> ReadLinesAsync(string path)
        {
            foreach (var line in _data) yield return line;
        }

        public async ValueTask<bool> WriteLinesAsync(string path, IEnumerable<string> data,
            CancellationToken cancellationToken = default)
        {
            return await Task.FromResult(true).ConfigureAwait(false);
        }
    }
}