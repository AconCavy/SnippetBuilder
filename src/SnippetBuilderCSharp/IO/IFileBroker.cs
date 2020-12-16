using System.Collections.Generic;

namespace SnippetBuilderCSharp.IO
{
    public interface IFileBroker
    {
        bool ExistsFile(string path);
        bool ExistsDirectory(string path);
        IEnumerable<string> GetFilePaths(string path, string searchPattern);
        void CreateDirectory(string path);
    }
}