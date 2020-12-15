# SnippetBuilderCSharp

## Command line with arguments

```
dotnet run -p ./src/SnippetBuilderCSharp -- --name hello --paths ./HelloWorld.cs --output ./artifacts
SnippetBuilderCSharp.exe --name hello --paths ./HelloWorld.cs --output ./artifacts
```

### Use recipe file

```json
[
  {
    "name": "sample1",
    "paths": [
      "./file1.cs"
    ],
    "output": "./output/"
  },
  {
    "name": "sample2",
    "paths": [
      "./file2.cs",
      "./file3.cs"
    ],
    "output": "./output/"
  }
]
```

```
dotnet run -p ./src/SnippetBuilderCSharp -- --recipe ./recipe.json
```

## Interactive

```sh
dotnet run -p ./src/SnippetBuilderCSharp

Enter the target file or directory paths
Enter a blank to go to the next step
./HelloWorld.cs

Enter the output directory (default is ./SnippetBuilderArtifacts/)
./artifacts

Enter the output file name (no ext.) (default is snippets)
hello

Building...
Complete! Look ./artifacts
```

```sh
cat ./artifacts/hello.code-snippet
{
  "HelloWorld": {
    "scope": "csharp",
    "prefix": [
      "helloworld",
      "hw"
    ],
    "body": [
      "namespace AwesomeProject",
      "{",
      "    class Program",
      "    {",
      "        static void Main(string[] args)",
      "        {",
      "            Console.WriteLine(\u0022Hello World!\u0022);",
      "        }",
      "    }",
      "}"
    ]
  }
}

```

## Supported Editor

- Visual Studio Code
