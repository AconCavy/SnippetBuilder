using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SnippetBuilderCSharp.Commands;
using SnippetBuilderCSharp.IO;
using SnippetBuilderCSharp.Models;
using SnippetBuilderCSharp.Snippets;

namespace SnippetBuilderCSharp
{
    public static class Program
    {
        private const string DefaultOutputDirectory = "./SnippetBuilderArtifacts/";
        private const string DefaultOutputName = "snippets";

        private static async Task Main(string[]? args)
        {
            var commandBuilder = new CommandProvider();
            var fileBroker = new FileBroker();
            var fileStreamBroker = new FileStreamBroker();

            if (args is null || !args.Any())
            {
                await InteractAsync(fileStreamBroker, fileBroker);
                return;
            }

            commandBuilder.RegisterCommand(new HelpCommand());
            commandBuilder.RegisterCommand(new NameCommand());
            commandBuilder.RegisterCommand(new PathsCommand(fileBroker));
            commandBuilder.RegisterCommand(new OutputCommand());
            commandBuilder.RegisterCommand(new RecipeCommand(fileStreamBroker, fileBroker));
            commandBuilder.Build(args);

            var recipes = new List<Recipe>();

            if (commandBuilder.TryResolveCommand<RecipeCommand>(out var recipeCommand))
                if (recipeCommand.Validate())
                    await foreach (var recipe in recipeCommand.GetRecipesAsync())
                        recipes.Add(recipe);

            var hasName = commandBuilder.TryResolveCommand<NameCommand>(out var nameCommand);
            var hasOutput = commandBuilder.TryResolveCommand<OutputCommand>(out var outputCommand);
            var hasPaths = commandBuilder.TryResolveCommand<PathsCommand>(out var pathsCommand);

            if (hasName && hasOutput && hasPaths)
            {
                var recipe = new Recipe();
                nameCommand.ApplyTo(recipe);
                outputCommand.ApplyTo(recipe);
                pathsCommand.ApplyTo(recipe);
                if (!nameCommand.Validate()) nameCommand.Append(DefaultOutputName);
                if (!outputCommand.Validate()) outputCommand.Append(DefaultOutputDirectory);
                if (nameCommand.Validate() && outputCommand.Validate() && pathsCommand.Validate()) recipes.Add(recipe);
            }

            foreach (var recipe in recipes)
            {
                if (recipe.Paths is null || !recipe.Paths.Any()) continue;
                recipe.Output ??= DefaultOutputDirectory;
                recipe.Name ??= DefaultOutputName;
                await new VisualStudioCodeSnippet(recipe, fileStreamBroker, fileBroker)
                    .BuildAsync();
            }
        }

        private static async ValueTask InteractAsync(IFileStreamBroker fileStreamBroker, IFileBroker fileBroker)
        {
            static void Close()
            {
                Console.WriteLine("Enter any key to close");
                _ = Console.ReadLine();
            }

            var recipe = new Recipe();
            var paths = new List<string>();
            Console.WriteLine("Enter the target file or directory paths");
            Console.WriteLine("Enter a blank to go to the next step");
            string? line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
                paths.AddRange(line.Split(" "));

            recipe.Paths = paths.Where(x => File.Exists(x) || Directory.Exists(x)).ToArray();
            if (recipe.Paths.Any())
            {
                Console.WriteLine("No valid file or directory paths");
                Close();
            }

            Console.WriteLine($"Enter the output directory (default is {DefaultOutputDirectory})");
            var outputDirectory = Console.ReadLine();
            recipe.Output = string.IsNullOrEmpty(outputDirectory) ? DefaultOutputDirectory : outputDirectory;
            Console.WriteLine();

            Console.WriteLine($"Enter the output file name (no ext.) (default is {DefaultOutputName})");
            var outputName = Console.ReadLine();
            recipe.Name = string.IsNullOrEmpty(outputName) ? DefaultOutputName : outputName;
            Console.WriteLine();

            Console.WriteLine("Building...");
            await new VisualStudioCodeSnippet(recipe, fileStreamBroker, fileBroker).BuildAsync();

            Console.WriteLine($"Complete! Look {Path.GetFullPath(recipe.Output)}");
            Close();
        }
    }
}