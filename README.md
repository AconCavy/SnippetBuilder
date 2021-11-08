# SnippetBuilder

## Requirement

- .NET 6.0

### Application runtimes

The released applications are framework-dependent on .NET 6.0, targeting Windows x64, Linux x64, and macOS x64.

## Command line with arguments

```sh
./SnippetBuilder --name hello --input ./HelloWorld.cs --extensions .cs --output ./artifacts
```

### Use recipe file

```json
[
  {
    "name": "sample1",
    "input": [
      "./file1.cs"
    ],
    "output": "./output/",
    "extensions": [
      ".cs"
    ]
  },
  {
    "name": "sample2",
    "input": [
      "./file2.cs",
      "./file3.cs"
    ],
    "output": "./output/",
    "extensions": [
      ".cs"
    ]
  }
]
```

```sh
./SnippetBuilder --recipes ./recipe.json
```

## Interactive

```sh
./SnippetBuilder

Enter the target file or directory paths
Enter a blank to go to the next step
./HelloWorld.cs

Enter a target file extensions
.cs

Enter the output directory (default is ./SnippetBuilderArtifacts/)
./artifacts

Enter the output file name (no ext.) (default is snippets)
hello

Building...
Complete!
```

```sh
cat ./artifacts/hello.code-snippet
{
  "HelloWorld": {
    "prefix": [
      "helloworld",
      "hw"
    ],
    "body": [
      "using System;",
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
