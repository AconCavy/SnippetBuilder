using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SnippetBuilder.IO;
using SnippetBuilder.Snippets;

namespace SnippetBuilder;

public static class Program
{
    private static async Task<int> Main(string[]? args)
    {
        args ??= Array.Empty<string>();
        var configuration = CreateHostBuilder(args).Build();
        using var scope = configuration.Services.CreateScope();
        var provider = scope.ServiceProvider;
        var runner = provider.GetRequiredService<Runner>();
        if (runner is null) throw new InvalidOperationException(nameof(runner));

        return await runner.RunAsync(args);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddScoped<IFileProvider, FileProvider>()
                    .AddScoped<IFileStreamProvider, FileStreamProvider>()
                    .AddScoped<ISnippet, VisualStudioCodeSnippet>()
                    .AddScoped<IRecipeSerializer, RecipeSerializer>()
                    .AddScoped<Runner>();
            });
}