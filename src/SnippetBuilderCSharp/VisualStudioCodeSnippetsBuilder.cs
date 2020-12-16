using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SnippetBuilderCSharp.IO;

namespace SnippetBuilderCSharp
{
    public class VisualStudioCodeSnippetsBuilder : SnippetsBuilderBase
    {
        protected override string Extension { get; } = ".code-snippets";
        private readonly Dictionary<string, Snippet> _dictionary;

        private class Snippet
        {
            [JsonPropertyName("scope")] public string Scope { get; set; } = "csharp";
            [JsonPropertyName("prefix")] public string[] Prefix { get; set; } = Array.Empty<string>();
            [JsonPropertyName("body")] public string[] Body { get; set; } = Array.Empty<string>();
        }

        public VisualStudioCodeSnippetsBuilder(Recipe recipe, IFileStreamBroker fileStreamBroker,
            IFileBroker fileBroker) : base(recipe, fileStreamBroker, fileBroker)
        {
            _dictionary = new Dictionary<string, Snippet>();
        }

        public override async ValueTask<IEnumerable<string>> BuildSnippetsAsync(
            CancellationToken cancellationToken = default)
        {
            foreach (var path in FilePaths)
            {
                var (section, snippet) = await CreateSnippetAsync(path, cancellationToken).ConfigureAwait(false);
                _dictionary[section] = snippet;
                if (cancellationToken.IsCancellationRequested) break;
            }

            var options = new JsonSerializerOptions {WriteIndented = true};
            var snippets = JsonSerializer.Serialize(_dictionary, options);
            return new[] {snippets};
        }

        private async ValueTask<(string, Snippet)> CreateSnippetAsync(string path, CancellationToken cancellationToken)
        {
            var name = Path.GetFileNameWithoutExtension(path);
            var skip = true;
            var body = new List<string>();
            await foreach (var line in FileStreamBroker.ReadLinesAsync(path).WithCancellation(cancellationToken))
            {
                if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;
                if (skip && line.Contains("using")) continue;
                skip = false;
                body.Add(line);
            }

            var prefixes = new List<string> {name.ToLower()};
            var abbreviation = new Regex("[a-z0-9]").Replace(name, "").ToLower();
            if (abbreviation.Length > 1) prefixes.Add(abbreviation);

            return (name, new Snippet {Prefix = prefixes.ToArray(), Body = body.ToArray()});
        }
    }
}