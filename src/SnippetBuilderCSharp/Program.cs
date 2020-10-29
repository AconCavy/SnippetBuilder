using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SnippetBuilderCSharp
{
    public static class Program
    {
        private const string DefaultOutputDirectory = "./SnippetBuilderArtifacts/";
        private const string DefaultOutputName = "snippets";

        private static async Task Main()
        {
            Console.WriteLine("Enter the target file or directory paths");
            Console.WriteLine("Enter a blank to go to the next step");
            var targets = new List<string>();
            while (true)
            {
                var target = Console.ReadLine();
                if (string.IsNullOrEmpty(target)) break;
                targets.AddRange(target.Split(" "));
            }

            targets = targets.Where(x => File.Exists(x) || Directory.Exists(x)).ToList();
            if (!targets.Any())
            {
                Console.WriteLine("No valid file or directory paths");
                Close();
            }

            Console.WriteLine($"Enter the output directory (default is {DefaultOutputDirectory})");
            var outputDirectory = Console.ReadLine();
            if (string.IsNullOrEmpty(outputDirectory)) outputDirectory = DefaultOutputDirectory;
            Console.WriteLine();

            Console.WriteLine($"Enter the output file name (no ext.) (default is {DefaultOutputName})");
            var outputName = Console.ReadLine();
            if (string.IsNullOrEmpty(outputName)) outputName = DefaultOutputName;
            Console.WriteLine();

            Console.WriteLine("Building...");
            await new VisualStudioCodeSnippetsBuilder(targets, outputDirectory, outputName).BuildAsync();

            Console.WriteLine($"Complete! Look {outputDirectory}");

            Close();
        }

        private static void Close()
        {
            Console.WriteLine("Enter any key to close");
            _ = Console.ReadLine();
        }
    }
}