using System.Collections.Generic;
using System.IO;

namespace SnippetBuilder.IO
{
    public class FileBroker : IFileBroker
    {
        public bool ExistsFile(string path) => File.Exists(path);

        public bool ExistsDirectory(string path) => Directory.Exists(path);

        public IEnumerable<string> GetFilePaths(string path, string searchPattern) =>
            Directory.GetFiles(path, searchPattern);

        public void CreateDirectory(string path) => Directory.CreateDirectory(path);
    }
}