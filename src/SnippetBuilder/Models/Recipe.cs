using System.Text.Json.Serialization;

namespace SnippetBuilder.Models
{
    public class Recipe
    {
        [JsonPropertyName("name")] public string? Name { get; set; }
        [JsonPropertyName("output")] public string? Output { get; set; }
        [JsonPropertyName("input")] public string[]? Input { get; set; }
        [JsonPropertyName("extensions")] public string[]? Extensions { get; set; }
    }
}