﻿using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using SnippetBuilderCSharp.Commands;

namespace SnippetBuilderCSharp
{
    public static class Program
    {
        private const string DefaultOutputDirectory = "./SnippetBuilderArtifacts/";
        private const string DefaultOutputName = "snippets";

        private static async Task Main(string[]? args)
        {
            var commandBuilder = new CommandProvider();
            var interact = args is null || !args.Any();

            if (interact)
            {
                Interact(commandBuilder);
            }
            else
            {
                commandBuilder.Register(new HelpCommand());
                commandBuilder.Register(new NameCommand());
                commandBuilder.Register(new PathsCommand());
                commandBuilder.Register(new OutputCommand());
                commandBuilder.Register(new RecipeCommand());
                commandBuilder.Build(args);

                var recipe = commandBuilder.Resolve<RecipeCommand>() ?? new RecipeCommand();
                if (recipe.Validate())
                {
                    foreach (var path in recipe.Parameters) await BuildFromConfigurationAsync(path);
                    return;
                }
            }

            var name = commandBuilder.Resolve<NameCommand>() ?? new NameCommand();
            name.Add(DefaultOutputName);
            if (!name.Validate()) throw new ArgumentException("name");
            var output = commandBuilder.Resolve<OutputCommand>() ?? new OutputCommand();
            output.Add(DefaultOutputDirectory);
            if (!output.Validate()) throw new ArgumentException("output");
            var paths = commandBuilder.Resolve<PathsCommand>() ?? new PathsCommand();
            if (!paths.Validate()) throw new ArgumentException("paths");

            var targets = paths.Parameters;
            var outputDirectory = output.Parameter;
            var outputName = name.Parameter;

            if (interact) Console.WriteLine("Building...");
            await new VisualStudioCodeSnippetsBuilder(targets, outputDirectory, outputName).BuildAsync();

            if (interact)
            {
                Console.WriteLine($"Complete! Look {Path.GetFullPath(outputDirectory)}");
                Close();
            }
        }

        private static void Interact(CommandProvider commandProvider)
        {
            var paths = new PathsCommand();
            Console.WriteLine("Enter the target file or directory paths");
            Console.WriteLine("Enter a blank to go to the next step");
            string? line;
            while (!string.IsNullOrEmpty(line = Console.ReadLine()))
                foreach (var path in line.Split(" "))
                    paths.Add(path);

            if (!paths.Validate())
            {
                Console.WriteLine("No valid file or directory paths");
                Close();
            }

            var output = new OutputCommand();
            Console.WriteLine($"Enter the output directory (default is {DefaultOutputDirectory})");
            var outputDirectory = Console.ReadLine();
            if (!string.IsNullOrEmpty(outputDirectory)) output.Add(outputDirectory);
            Console.WriteLine();

            var name = new NameCommand();
            Console.WriteLine($"Enter the output file name (no ext.) (default is {DefaultOutputName})");
            var outputName = Console.ReadLine();
            if (!string.IsNullOrEmpty(outputName)) name.Add(outputName);
            Console.WriteLine();

            commandProvider.Register(paths);
            commandProvider.Register(output);
            commandProvider.Register(name);
        }

        private static void Close()
        {
            Console.WriteLine("Enter any key to close");
            _ = Console.ReadLine();
        }

        private static async ValueTask BuildFromConfigurationAsync(string path)
        {
            await using var stream = new FileStream(path, FileMode.Open);
            var recipes = await JsonSerializer.DeserializeAsync<Recipe[]>(stream);
            foreach (var recipe in recipes)
            {
                var targets = recipe.Paths ?? Array.Empty<string>();
                var outputDirectory = recipe.Output ?? DefaultOutputDirectory;
                var outputName = recipe.Name ?? DefaultOutputName;
                await new VisualStudioCodeSnippetsBuilder(targets, outputDirectory, outputName)
                    .BuildAsync();
            }
        }
    }
}