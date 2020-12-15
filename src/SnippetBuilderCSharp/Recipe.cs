using System.Text.Json.Serialization;

namespace SnippetBuilderCSharp
{
    public class Recipe
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("output")] public string? Output { get; set; }
        [JsonPropertyName("paths")] public string[]? Paths { get; set; }
    }
}