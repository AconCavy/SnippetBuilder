﻿using System;
using System.Collections.Generic;
using SnippetBuilderCSharp.IO;

namespace SnippetBuilderCSharp.Test.Fakes
{
    public class FakeFileBroker : IFileBroker
    {
        public bool ExistsFile(string path)
        {
            return path.EndsWith(".cs");
        }

        public bool ExistsDirectory(string path)
        {
            return !path.EndsWith(".*");
        }

        public IEnumerable<string> GetFilePaths(string path, string searchPattern)
        {
            return Array.Empty<string>();
        }

        public void CreateDirectory(string path)
        {
        }
    }
}