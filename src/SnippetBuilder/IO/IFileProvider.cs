using System.Collections.Generic;

namespace SnippetBuilder.IO;

public interface IFileProvider
{
    bool ExistsFile(string path);
    bool ExistsDirectory(string path);
    IEnumerable<string> GetFilePaths(string path, string searchPattern);
    void CreateDirectory(string path);
}