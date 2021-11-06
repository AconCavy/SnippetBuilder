using System;
using System.Collections.Generic;
using SnippetBuilder.IO;

namespace SnippetBuilder.Test.Fakes;

public class FakeFileBroker : IFileBroker
{
    public bool ExistsFile(string path) => path.EndsWith(".cs");

    public bool ExistsDirectory(string path) => !path.EndsWith(".*");

    public IEnumerable<string> GetFilePaths(string path, string searchPattern) => Array.Empty<string>();

    public void CreateDirectory(string path)
    {
    }
}