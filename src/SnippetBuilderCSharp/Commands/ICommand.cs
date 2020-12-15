using System.Collections.Generic;

namespace SnippetBuilderCSharp.Commands
{
    public interface ICommand
    {
        string Parameter { get; }
        IEnumerable<string> Parameters { get; }
        bool Validate();
    }
}