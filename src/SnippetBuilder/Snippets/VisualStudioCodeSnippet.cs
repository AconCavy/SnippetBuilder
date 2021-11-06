using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using SnippetBuilder.IO;

namespace SnippetBuilder.Snippets;

public class VisualStudioCodeSnippet : SnippetBase
{
    public VisualStudioCodeSnippet(IFileStreamProvider fileStreamProvider, IFileProvider fileProvider)
        : base(fileStreamProvider, fileProvider)
    {
    }

    protected override string Extension { get; } = ".code-snippets";

    public override async Task<IEnumerable<string>> BuildAsync(IEnumerable<string> paths,
        CancellationToken cancellationToken = default)
    {
        var sections = new Dictionary<string, Section>();
        foreach (var path in paths)
        {
            if (!FileProvider.ExistsFile(path)) continue;
            var (title, section) = await CreateSectionAsync(path, cancellationToken).ConfigureAwait(false);
            sections[title] = section;
            if (cancellationToken.IsCancellationRequested) break;
        }

        var options = new JsonSerializerOptions { WriteIndented = true };
        var snippets = JsonSerializer.Serialize(sections, options);
        return new[] { snippets };
    }

    private async ValueTask<(string, Section)> CreateSectionAsync(string path,
        CancellationToken cancellationToken)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        var body = new List<string>();
        await foreach (var line in FileStreamProvider.ReadLinesAsync(path).WithCancellation(cancellationToken))
        {
            if (string.IsNullOrEmpty(line) || string.IsNullOrWhiteSpace(line)) continue;
            body.Add(line);
        }

        var prefixes = new List<string> { name.ToLower() };
        var abbreviation = new Regex("[a-z0-9]").Replace(name, "").ToLower();
        if (abbreviation.Length > 1) prefixes.Add(abbreviation);

        return (name, new Section { Prefix = prefixes.ToArray(), Body = body.ToArray() });
    }

    private class Section
    {
        [JsonPropertyName("prefix")] public string[] Prefix { get; set; } = Array.Empty<string>();
        [JsonPropertyName("body")] public string[] Body { get; set; } = Array.Empty<string>();
    }
}