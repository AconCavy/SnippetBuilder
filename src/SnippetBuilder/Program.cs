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
        var runner = configuration.Services.GetService<Runner>();
        if (runner is null) throw new InvalidOperationException(nameof(runner));

        return await runner.RunAsync(args);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddScoped<IFileProvider, FileProvider>();
                services.AddScoped<IFileStreamProvider, FileStreamProvider>();
                services.AddScoped<ISnippet, VisualStudioCodeSnippet>();
                services.AddScoped<IRecipeSerializer, RecipeSerializer>();

                services.AddSingleton<Runner>();
            });
}