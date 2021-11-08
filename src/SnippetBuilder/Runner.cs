using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading.Tasks;
using SnippetBuilder.IO;
using SnippetBuilder.Models;
using SnippetBuilder.Snippets;

namespace SnippetBuilder;

public class Runner
{
    private const string DefaultOutputDirectory = "./SnippetBuilderArtifacts/";
    private const string DefaultOutputName = "snippets";
    private readonly Dictionary<string, int> _counts;
    private readonly IRecipeSerializer _recipeSerializer;

    private readonly ISnippet[] _snippets;

    public Runner(IEnumerable<ISnippet> snippets, IRecipeSerializer recipeSerializer)
    {
        _snippets = snippets.ToArray();
        _recipeSerializer = recipeSerializer;
        _counts = new Dictionary<string, int>();
    }

    public Task<int> RunAsync(string[] args)
    {
        var inputOption = new Option<string[]>("--input", Array.Empty<string>, "Input file or directory paths");
        inputOption.AddAlias("-i");
        var outputOption = new Option<string>("--output", static () => DefaultOutputDirectory, "Output directory path");
        outputOption.AddAlias("-o");
        var nameOption = new Option<string>("--name", static () => DefaultOutputName, "Output file name");
        nameOption.AddAlias("-n");
        var extensionsOption = new Option<string[]>("--extensions", Array.Empty<string>, "Include file extensions");
        extensionsOption.AddAlias("-ext");
        var recipesOption = new Option<string[]>("--recipes", Array.Empty<string>, "Recipe file paths");
        recipesOption.AddAlias("-r");

        var command = new RootCommand
        {
            inputOption,
            outputOption,
            nameOption,
            extensionsOption,
            recipesOption
        };
        command.Description = "SnippetBuilder is an editor snippet building tool.";
        command.Handler = CommandHandler.Create<string[], string, string, string[], string[]>(RunAsync);

        return command.InvokeAsync(args);
    }

    private async Task RunAsync(string[] input, string output, string name, string[] extensions, string[] recipes)
    {
        var targets = new List<Recipe>();

        if (input.Length == 0 && recipes.Length == 0)
        {
            targets.Add(CreateRecipe());
        }
        else if (input.Length > 0)
        {
            targets.Add(CreateRecipe(input, output, name, extensions));
        }

        targets.AddRange(await CreateRecipesAsync(recipes));

        Console.WriteLine("Building...");
        await BuildSnippetsAsync(targets).ConfigureAwait(false);
        Console.WriteLine("Complete!");
    }

    private Recipe CreateRecipe(string[] input, string output, string name, string[] extensions)
    {
        var recipe = new Recipe { Input = input, Name = name, Output = output, Extensions = extensions };
        var count = IncrementCount(recipe.Name);
        if (count > 1) recipe.Name += $"_{count - 1}";
        return recipe;
    }

    private Recipe CreateRecipe()
    {
        static string? GetInput()
        {
            return Console.ReadLine()?.Trim();
        }

        static IEnumerable<string> GetInputs()
        {
            string? line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
            {
                foreach (var item in line.Split(" "))
                {
                    yield return item;
                }
            }
        }

        var recipe = new Recipe();
        do
        {
            Console.WriteLine("Enter a target file or directory paths");
            Console.WriteLine("Enter a blank to go to the next step");
            recipe.Input = GetInputs().ToArray();
            Console.WriteLine();
        } while (!recipe.Input.Any());

        Console.WriteLine("Enter a target file extensions");
        recipe.Extensions = GetInputs().ToArray();
        Console.WriteLine();

        Console.WriteLine($"Enter a output directory (default is {DefaultOutputDirectory})");
        var outputDirectory = GetInput();
        recipe.Output = string.IsNullOrEmpty(outputDirectory) ? DefaultOutputDirectory : outputDirectory;
        Console.WriteLine();

        Console.WriteLine($"Enter a output file name (no ext.) (default is {DefaultOutputName})");
        var outputName = GetInput();
        recipe.Name = string.IsNullOrEmpty(outputName) ? DefaultOutputName : outputName;
        Console.WriteLine();

        return recipe;
    }

    private async Task<IEnumerable<Recipe>> CreateRecipesAsync(IEnumerable<string> paths)
    {
        var recipes = new List<Recipe>();
        await foreach (var recipe in _recipeSerializer.DeserializeAsync(paths))
        {
            recipe.Output ??= DefaultOutputDirectory;
            recipe.Name ??= DefaultOutputName;
            var count = IncrementCount(recipe.Name);
            if (count > 1) recipe.Name += $"_{count - 1}";
            recipes.Add(recipe);
        }

        return recipes;
    }

    private Task BuildSnippetsAsync(IEnumerable<Recipe> recipes)
    {
        var tasks = new List<Task>();
        foreach (var recipe in recipes)
        {
            foreach (var snippet in _snippets)
            {
                tasks.Add(snippet.BuildAsync(recipe));
            }
        }

        return Task.WhenAll(tasks);
    }

    private int IncrementCount(string key)
    {
        if (!_counts.ContainsKey(key)) _counts[key] = 0;
        return ++_counts[key];
    }
}