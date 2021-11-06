using System.Collections.Generic;
using System.IO;

namespace SnippetBuilder.IO;

internal class FileBroker : IFileBroker
{
    public bool ExistsFile(string path)
    {
        return File.Exists(path);
    }

    public bool ExistsDirectory(string path)
    {
        return Directory.Exists(path);
    }

    public IEnumerable<string> GetFilePaths(string path, string searchPattern)
    {
        return Directory.GetFiles(path, searchPattern);
    }

    public void CreateDirectory(string path)
    {
        Directory.CreateDirectory(path);
    }
}