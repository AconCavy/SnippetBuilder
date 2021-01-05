using System.Collections.Generic;

namespace SnippetBuilder.Commands
{
    public interface ICommand
    {
        string Parameter { get; }
        IEnumerable<string> Parameters { get; }
        bool Validate();
        void Append(string parameter);
    }
}