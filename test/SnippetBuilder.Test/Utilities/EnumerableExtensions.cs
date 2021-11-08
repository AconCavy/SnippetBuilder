namespace SnippetBuilder.Test.Utilities;

public static class EnumerableExtensions
{
    public static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(this IEnumerable<T> source)
    {
        if (source is null) throw new ArgumentNullException(nameof(source));
        foreach (var item in source) yield return await Task.FromResult(item);
    }
}